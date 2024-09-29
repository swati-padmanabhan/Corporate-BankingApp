using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CorporateBankingApp.Services
{
    public interface IClientService
    {
        List<Employee> GetAllEmployees(Guid clientId);

        Employee MapToEmployee(EmployeeDTO employeeDTO, Client client);

        void UploadEmployeeCsv(HttpPostedFileBase csvFile, Client client);

        void AddEmployeeDetails(EmployeeDTO employeeDTO, Client client);

        Employee GetEmployeeById(Guid id);

        Client GetClientById(Guid clientId);

        void UpdateEmployeeDetails(EmployeeDTO employeeDTO, Client client);

        void UpdateEmployeeStatus(Guid id, bool isActive);

        List<Beneficiary> GetAllBeneficiaries(Guid clientId);



    }
}
