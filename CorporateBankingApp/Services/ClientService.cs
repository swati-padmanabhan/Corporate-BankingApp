using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
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

        //public Employee MapToEmployee(EmployeeDTO employeeDTO, Client client)
        //{
        //    return new Employee
        //    {
        //        Id = employeeDTO.Id == Guid.Empty ? Guid.NewGuid() : employeeDTO.Id,
        //        FirstName = employeeDTO.FirstName,
        //        LastName = employeeDTO.LastName,
        //        Email = employeeDTO.Email,
        //        Phone = employeeDTO.Phone,
        //        Designation = employeeDTO.Designation,
        //        Salary = employeeDTO.Salary,
        //        IsActive = employeeDTO.IsActive,
        //        Client = client
        //    };
        //}

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
                        Salary = employeeDTO.Salary,
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
                Salary = employeeDTO.Salary,
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
                existingEmployee.Salary = employeeDTO.Salary;
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


        //salary disbursement code 
        public bool ProcessSalaryDisbursements(List<Guid> employeeIds, bool isBatch, out List<Guid> excludedEmployees)
        {
            excludedEmployees = new List<Guid>();

            var employees = _clientRepository.RetrieveEmployeesByIds(employeeIds);

            if (employees == null || !employees.Any())
            {
                return false;
            }

            foreach (var employee in employees)
            {
                // Check if a salary has already been disbursed to this employee on this date
                if (HasSalaryBeenDisbursed(employee.Id))
                {
                    excludedEmployees.Add(employee.Id);
                    continue;
                }

                var salaryDisbursement = CreateSalaryDisbursement(employee, isBatch);
                _clientRepository.AddSalaryDisbursement(salaryDisbursement);
            }

            return true;
        }

        // Helper method to check for existing salary disbursement
        private bool HasSalaryBeenDisbursed(Guid employeeId)
        {
            var existingDisbursement = _clientRepository.GetEmployeeSalaryDisbursement(employeeId, DateTime.Now);
            return existingDisbursement != null;
        }

        // Helper method to create a new SalaryDisbursement
        private SalaryDisbursement CreateSalaryDisbursement(Employee employee, bool isBatch)
        {
            return new SalaryDisbursement
            {
                Employee = employee,
                DisbursementDate = DateTime.Now,
                IsBatch = isBatch,
                SalaryStatus = CompanyStatus.PENDING
            };
        }

    }
}