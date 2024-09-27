using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CorporateBankingApp.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }
        public void AddEmployeeDetails(EmployeeDTO employeeDTO, Client client)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = employeeDTO.FirstName,
                LastName = employeeDTO.LastName,
                Email = employeeDTO.Email,
                Phone = employeeDTO.Phone,
                Designation = employeeDTO.Designation,
                IsActive = true,
                Client = client
            };
            _clientRepository.AddEmployeeDetails(employee);
        }

        public List<Employee> GetAllEmployees(Guid clientId)
        {
            return _clientRepository.GetAllEmployees(clientId);
        }

        public Client GetClientById(Guid clientId)
        {
            return _clientRepository.GetClientById(clientId);

        }

        public Employee GetEmployeeById(Guid id)
        {
            return _clientRepository.GetEmployeeById(id);
        }


        public void UpdateEmployeeDetails(EmployeeDTO employeeDTO, Client client)
        {
            var existingEmployee = _clientRepository.GetEmployeeById(employeeDTO.Id);

            if (existingEmployee != null)
            {
                existingEmployee.FirstName = employeeDTO.FirstName;
                existingEmployee.LastName = employeeDTO.LastName;
                existingEmployee.Email = employeeDTO.Email;
                existingEmployee.Phone = employeeDTO.Phone;
                existingEmployee.Designation = employeeDTO.Designation;
                existingEmployee.Client = client;
                _clientRepository.UpdateEmployeeDetails(existingEmployee);
            }
        }

        public void UpdateEmployeeStatus(Guid id, bool isActive)
        {
            _clientRepository.UpdateEmployeeStatus(id, isActive);
        }


    }
}