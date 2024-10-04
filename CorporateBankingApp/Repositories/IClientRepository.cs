using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Repositories
{
    public interface IClientRepository
    {
        void UpdateClientBalance(Guid clientId, double newBalance);

        void EditClientRegistration(Client client);

        bool EmailExists(string email);

        List<Employee> GetAllEmployees(Guid clientId);

        void AddEmployeeDetails(Employee employee);

        Employee GetEmployeeById(Guid id);

        Client GetClientById(Guid clientId);

        void UpdateEmployeeDetails(Employee employee);

        void UpdateEmployeeStatus(Guid id, bool isActive);

        List<Beneficiary> GetAllBeneficiaries(Guid clientId);

        //salary  disbursement
        List<Employee> RetrieveEmployeesByIds(List<Guid> employeeIds);

        void AddSalaryDisbursement(SalaryDisbursement salaryDisbursement);

        SalaryDisbursement GetEmployeeSalaryDisbursement(Guid employeeId, DateTime currentDate);

        List<Beneficiary> GetBeneficiaryList(Guid clientId);

    }
}
