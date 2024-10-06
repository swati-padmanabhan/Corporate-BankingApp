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

        //verify outbound beneficiary
        public List<Beneficiary> GetPendingBeneficiaries()
        {
            var beneficiaries = _session.Query<Beneficiary>().FetchMany(c => c.Documents).Where(b => b.BeneficiaryStatus == CompanyStatus.PENDING && b.BeneficiaryType == BeneficiaryType.OUTBOUND).ToList();
            return beneficiaries;
        }
        public Beneficiary GetBeneficiaryById(Guid id)
        {
            return _session.Get<Beneficiary>(id);
        }
        public void UpdateBeneficiary(Beneficiary beneficiary)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(beneficiary);
                transaction.Commit();
            }
        }

        //verify payments
        public IEnumerable<PaymentDTO> GetPendingPaymentsByStatus(CompanyStatus status)
        {

            return _session.Query<Payment>()
                .Where(x => x.PaymentStatus == status)
                .OrderByDescending(x => x.PaymentRequestDate)
                .Select(x => new PaymentDTO
                {
                    PaymentId = x.Id,
                    CompanyName = x.Beneficiary.BeneficiaryName,
                    AccountNumber = x.Beneficiary.AccountNumber,
                    BeneficiaryType = x.Beneficiary.BeneficiaryType,
                    Amount = x.Amount,
                    PaymentRequestDate = x.PaymentRequestDate
                })
                .ToList();
        }

        public void UpdatePaymentStatus(Guid paymentId, CompanyStatus status)
        {
            var payment = _session.Get<Payment>(paymentId);



            if (payment != null)
            {
                payment.PaymentStatus = status;
                if (status == CompanyStatus.APPROVED)
                {
                    payment.PaymentApprovalDate = DateTime.Now;
                    var client = _session.Get<Client>(payment.ClientId);

                    if (client != null)
                    {
                        client.Balance -= payment.Amount;
                        _session.Update(client);
                    }
                }
                _session.Update(payment);
                _session.Flush(); // Save changes to the database
            }
        }

        public Payment GetPaymentById(Guid id)
        {
            return _session.Query<Payment>().FirstOrDefault(p => p.Id == id);
        }

        public void UpdatePayment(Payment payment)
        {
            using (ITransaction transaction = _session.BeginTransaction())
            {
                _session.Update(payment);
                transaction.Commit();
            }
        }
    }
}
