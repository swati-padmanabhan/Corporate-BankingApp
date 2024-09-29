using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        public Employee MapToEmployee(EmployeeDTO employeeDTO, Client client)
        {
            return new Employee
            {
                Id = employeeDTO.Id == Guid.Empty ? Guid.NewGuid() : employeeDTO.Id,
                FirstName = employeeDTO.FirstName,
                LastName = employeeDTO.LastName,
                Email = employeeDTO.Email,
                Phone = employeeDTO.Phone,
                Designation = employeeDTO.Designation,
                IsActive = employeeDTO.IsActive,
                Client = client
            };
        }

        public void UploadEmployeeCsv(HttpPostedFileBase csvFile, Client client)
        {
            // Save the CSV file locally
            string folderPath = HttpContext.Current.Server.MapPath("~/Content/Documents/EmployeeCSV/") + client.UserName;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = Path.GetFileName(csvFile.FileName);
            string filePath = Path.Combine(folderPath, fileName);
            csvFile.SaveAs(filePath);

            // Read the CSV and add employee details
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,  // Ignore header validation
                MissingFieldFound = null, // Ignore missing fields like 'Id' and 'IsActive'
                PrepareHeaderForMatch = args => args.Header.ToLower() // Make headers case-insensitive
            }))
            {
                // Fetch the employee records from the CSV file
                var employeeRecords = csv.GetRecords<EmployeeDTO>().ToList();

                foreach (var employeeDTO in employeeRecords)
                {
                    // Map EmployeeDTO to Employee entity
                    var employee = new Employee
                    {
                        Id = Guid.NewGuid(),  // Generate new Guid for each employee
                        FirstName = employeeDTO.FirstName,
                        LastName = employeeDTO.LastName,
                        Email = employeeDTO.Email,
                        Phone = employeeDTO.Phone,
                        Designation = employeeDTO.Designation,
                        IsActive = true, // Assuming default status is active
                        Client = client  // Associate employee with the provided client
                    };

                    // Save employee details to the database
                    AddEmployeeDetails(employeeDTO, client);
                }
            }
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

        public List<Beneficiary> GetAllBeneficiaries(Guid clientId)
        {
            return _clientRepository.GetAllBeneficiaries(clientId);
        }
    }
}