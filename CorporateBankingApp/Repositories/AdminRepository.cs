using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Utils;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CorporateBankingApp.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ISession _session;

        public AdminRepository(ISession session)
        {
            _session = session;
        }

        public void CreateAdmin(Admin admin)
        {
            using (var transaction = _session.BeginTransaction())
            {
                admin.Password = PasswordHashing.HashPassword(admin.Password);
                var role = new Role
                {
                    RoleName = "Admin",
                    User = admin
                };
                _session.Save(admin);
                _session.Save(role);
                transaction.Commit();
            }
        }

        public Client GetClientById(Guid id)
        {
            return _session.Get<Client>(id);
        }

        public void DeleteClientDetails(Guid id)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var currentClient = _session.Get<Client>(id);

                if (currentClient != null)
                {
                    currentClient.IsActive = false;
                    _session.Update(currentClient);
                }
                transaction.Commit();
            }
        }

        public List<Client> GetAllClients()
        {
            return _session.Query<Client>()
                           .Where(c => c.IsActive)
                           .ToList();
        }

        public void UpdateClientDetails(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var existingClient = _session.Get<Client>(client.Id);

                if (existingClient != null)
                {
                    existingClient.UserName = client.UserName;
                    existingClient.Email = client.Email;
                    existingClient.CompanyName = client.CompanyName;
                    existingClient.Location = client.Location;
                    existingClient.ContactInformation = client.ContactInformation;

                    _session.Update(existingClient);
                }
                transaction.Commit();
            }
        }

        public List<Client> GetPendingClients()
        {
            var clients = _session.Query<Client>().FetchMany(c => c.Documents).Where(c => c.OnBoardingStatus == CompanyStatus.PENDING).ToList();
            return clients;
        }


        // Salary Disbursement 
        public SalaryDisbursement GetSalaryDisbursementById(Guid id)
        {
            return _session.Query<SalaryDisbursement>()
                           .Fetch(sd => sd.Employee) 
                           .ThenFetch(e => e.Client) 
                           .FirstOrDefault(sd => sd.Id == id);
        }

        public IEnumerable<SalaryDisbursementDTO> GetSalaryDisbursementsByStatus(CompanyStatus status)
        {
            return _session.Query<SalaryDisbursement>()
                           .Where(x => x.SalaryStatus == status)
                           .Select(x => new SalaryDisbursementDTO
                           {
                               SalaryDisbursementId = x.Id,
                               CompanyName = x.Employee.Client.CompanyName,
                               EmployeeFirstName = x.Employee.FirstName,
                               EmployeeLastName = x.Employee.LastName,
                               Salary = x.Employee.Salary,
                               DisbursementDate = x.DisbursementDate,
                               SalaryStatus = x.SalaryStatus
                           })
                           .ToList();
        }

        public bool ApproveSalaryDisbursement(Guid salaryDisbursementId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var salaryDisbursement = _session.Get<SalaryDisbursement>(salaryDisbursementId);
                    if (salaryDisbursement == null || salaryDisbursement.SalaryStatus != CompanyStatus.PENDING)
                        return false;

                    var employee = salaryDisbursement.Employee;
                    var client = employee.Client;

                    double totalSalary = employee.Salary;

                    if (client.Balance < totalSalary)
                    {
                        return false;
                    }

                    client.Balance -= totalSalary;
                    salaryDisbursement.SalaryStatus = CompanyStatus.APPROVED;

                    _session.Update(client);
                    _session.Update(salaryDisbursement);
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool RejectSalaryDisbursement(Guid salaryDisbursementId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var salaryDisbursement = _session.Get<SalaryDisbursement>(salaryDisbursementId);
                    if (salaryDisbursement == null || salaryDisbursement.SalaryStatus != CompanyStatus.PENDING)
                        return false;

                    salaryDisbursement.SalaryStatus = CompanyStatus.REJECTED;

                    _session.Update(salaryDisbursement);
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
