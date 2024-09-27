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
        List<Employee> GetAllEmployees(Guid clientId);

        void AddEmployeeDetails(Employee employee);

        Employee GetEmployeeById(Guid id);

        Client GetClientById(Guid clientId);

        void UpdateEmployeeDetails(Employee employee);

        void UpdateEmployeeStatus(Guid id, bool isActive);
    }
}
