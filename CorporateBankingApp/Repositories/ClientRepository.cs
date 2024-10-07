using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CorporateBankingApp.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ISession _session;

        public ClientRepository(ISession session)
        {
            _session = session;
        }

        public void UpdateClientBalance(Guid clientId, double newBalance)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var client = _session.Get<Client>(clientId);
                if (client != null)
                {
                    client.Balance = newBalance;  // Update the balance
                    _session.Update(client);
                    transaction.Commit();
                }
            }
        }

        //*******************************************Client reupload documents*******************************************

        public void EditClientRegistration(Client client)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(client);
                transaction.Commit();
            }
        }

        //*******************************************Employee*******************************************
        public bool EmailExists(string email)
        {
            return _session.Query<Employee>().Any(e => e.Email == email);
        }

        public void AddEmployeeDetails(Employee employee)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(employee);
                transaction.Commit();
            }
        }

        public List<Employee> GetAllEmployees(Guid clientId)
        {
            var client = _session.Query<Client>()
                                 .FetchMany(c => c.Employees)
                                 .SingleOrDefault(c => c.Id == clientId);
            return client?.Employees.ToList() ?? new List<Employee>();
        }

        public Employee GetEmployeeById(Guid id)
        {
            return _session.Get<Employee>(id);
        }

        public Client GetClientById(Guid clientId)
        {
            return _session.Get<Client>(clientId);
        }

        public void UpdateEmployeeDetails(Employee employee)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Update(employee);
                transaction.Commit();
            }
        }

        public void UpdateEmployeeStatus(Guid id, bool isActive)
        {
            using (var transaction = _session.BeginTransaction())
            {
                var employee = _session.Get<Employee>(id);
                if (employee != null)
                {
                    employee.IsActive = isActive;
                    _session.Update(employee);
                    transaction.Commit();
                }
            }
        }

        public List<Beneficiary> GetAllBeneficiaries(Guid clientId)
        {
            var client = _session.Query<Client>()
                                 .FetchMany(c => c.Beneficiaries)
                                 .SingleOrDefault(c => c.Id == clientId);
            return client?.Beneficiaries.ToList() ?? new List<Beneficiary>();
        }

        //******************************************salary******************************************

        public List<Employee> RetrieveEmployeesByIds(List<Guid> employeeIds)
        {
            return _session.Query<Employee>()
                   .Where(e => employeeIds.Contains(e.Id))
                   .ToList();
        }

        public void AddSalaryDisbursement(SalaryDisbursement salaryDisbursement)
        {
            using (var transaction = _session.BeginTransaction())
            {
                _session.Save(salaryDisbursement);
                transaction.Commit();
            }
        }

        public SalaryDisbursement GetEmployeeSalaryDisbursement(Guid employeeId, DateTime currentDate)
        {
            var (startOfMonth, endOfMonth) = GetMonthDateRange(currentDate);

            return _session.Query<SalaryDisbursement>()
                           .FirstOrDefault(sd => sd.Employee.Id == employeeId &&
                                                 sd.DisbursementDate >= startOfMonth &&
                                                 sd.DisbursementDate < endOfMonth);
        }

        private (DateTime start, DateTime end) GetMonthDateRange(DateTime date)
        {
            var start = new DateTime(date.Year, date.Month, 1);
            var end = start.AddMonths(1);
            return (start, end);
        }

        //payments
        public List<Beneficiary> GetBeneficiaryList(Guid clientId)
        {
            var client = GetClientById(clientId);
            var beneficiaries = _session.Query<Beneficiary>().Where(b => b.Client.Id == clientId && b.BeneficiaryStatus == CompanyStatus.APPROVED && b.IsActive == true).ToList();
            return beneficiaries;
        }
    }
}
