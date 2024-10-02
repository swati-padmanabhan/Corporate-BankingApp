using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CorporateBankingApp.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IEmailService _emailService;
        private readonly IClientRepository _clientRepository;
        private readonly IEmailRepository _emailRepository;

        public AdminService(IAdminRepository adminRepository, IEmailService emailService, IClientRepository clientRepository, IEmailRepository emailRepository)
        {
            _adminRepository = adminRepository;
            _emailService = emailService;
            _clientRepository = clientRepository;
            _emailRepository = emailRepository;
        }

        public void RegisterAdmin(AdminDTO adminDTO)
        {
            var admin = new Admin
            {
                UserName = adminDTO.UserName,
                Password = adminDTO.Password,
                Email = adminDTO.Email,
                BankName = adminDTO.BankName
            };

            _adminRepository.CreateAdmin(admin);
        }

        public List<ClientDTO> GetRegisteredClientsPendingApproval()
        {
            var clients = _adminRepository.GetAllClients()
                .Where(c => c.OnBoardingStatus == CompanyStatus.PENDING && c.IsActive).ToList();
            return clients.Select(c => new ClientDTO
            {
                Id = c.Id,
                UserName = c.UserName,
                Email = c.Email,
                CompanyName = c.CompanyName,
                Location = c.Location,
                ContactInformation = c.ContactInformation
            }).ToList();
        }

        public ClientDTO GetClientById(Guid id)
        {
            var client = _adminRepository.GetClientById(id);
            return new ClientDTO
            {
                Id = client.Id,
                UserName = client.UserName,
                Email = client.Email,
                CompanyName = client.CompanyName,
                Location = client.Location,
                ContactInformation = client.ContactInformation,
                AccountNumber = client.AccountNumber,
                ClientIFSC = client.ClientIFSC,
                Balance = client.Balance,
                DocumentLocation = client.Documents.Select(doc => doc.FilePath).ToList()
            };
        }


        public void ApproveClient(Guid id)
        {
            var client = _adminRepository.GetClientById(id);
            client.OnBoardingStatus = CompanyStatus.APPROVED;
            _adminRepository.UpdateClientDetails(client);
        }

        public void RejectClient(Guid id)
        {
            var client = _adminRepository.GetClientById(id);
            client.IsActive = false; // Set inactive if rejected
            _adminRepository.UpdateClientDetails(client);
        }

        public List<ClientDTO> GetClientsForVerification(UrlHelper urlHelper)
        {
            var clients = _adminRepository.GetPendingClients();
            var clientDtos = clients.Select(c => new ClientDTO
            {
                Id = c.Id,
                UserName = c.UserName,
                Email = c.Email,
                CompanyName = c.CompanyName,
                ContactInformation = c.ContactInformation,
                Location = c.Location,
                DocumentLocation = c.Documents.Select(d => urlHelper.Content(d.FilePath)).ToList()
            }).ToList();
            return clientDtos;
        }

        public bool UpdateClientOnboardingStatus(Guid id, string status)
        {
            var client = _adminRepository.GetClientById(id);
            if (client == null)
            {
                // Client not found
                return false;
            }
            // Update onboarding status based on the status string
            if (status == "APPROVED")
            {
                client.OnBoardingStatus = CompanyStatus.APPROVED;
                _emailRepository.SendEmailNotification(client.Email, "Client Approved!!", $"Dear {client.UserName}, Your client account has been approved as all submitted details and documents meet our onboarding requirements. Now you can access all our services.");
            }
            else if (status == "REJECTED")
            {
                client.OnBoardingStatus = CompanyStatus.REJECTED;
                _emailRepository.SendEmailNotification(client.Email, "Client Rejected!!", $"Dear {client.UserName}, Your client account has been rejected due to discrepancies in the submitted details and documents, which do not meet our onboarding requirements.");
            }
            _adminRepository.UpdateClientDetails(client);
            return true;
        }




        public void DeleteClientDetails(Guid id)
        {
            _adminRepository.DeleteClientDetails(id);
        }

        public List<Client> GetAllClients()
        {
            var clients = _adminRepository.GetAllClients();
            return clients;
        }

        public void UpdateClientDetails(ClientDTO clientDTO, Guid id)
        {
            var client = new Client
            {
                Id = id,
                UserName = clientDTO.UserName,
                Email = clientDTO.Email,
                CompanyName = clientDTO.CompanyName,
                Location = clientDTO.Location,
                ContactInformation = clientDTO.ContactInformation,
            };
            _adminRepository.UpdateClientDetails(client);
        }

        //salary

        //public SalaryDisbursementDTO GetSalaryDisbursementById(Guid salaryDisbursementId)
        //{
        //    var salaryDisbursement = _adminRepository.GetSalaryDisbursementById(salaryDisbursementId);
        //    if (salaryDisbursement == null)
        //    {
        //        return null; // or handle accordingly
        //    }

        //    return new SalaryDisbursementDTO
        //    {
        //        SalaryDisbursementId = salaryDisbursement.Id,
        //        CompanyName = salaryDisbursement.Employee.Client.CompanyName,
        //        EmployeeFirstName = salaryDisbursement.Employee.FirstName,
        //        EmployeeLastName = salaryDisbursement.Employee.LastName,
        //        Salary = salaryDisbursement.Employee.Salary,
        //        DisbursementDate = salaryDisbursement.DisbursementDate,
        //        SalaryStatus = salaryDisbursement.SalaryStatus
        //    };
        //}

        public IEnumerable<SalaryDisbursementDTO> ListPendingSalaryDisbursements()
        {
            return _adminRepository.GetSalaryDisbursementsByStatus(CompanyStatus.PENDING);
        }




        public bool ApproveSalaryDisbursement(Guid salaryDisbursementId, bool isBatch = false)
        {
            //return _adminRepository.ApproveSalaryDisbursement(salaryDisbursementId);
            var salaryDisbursement = _adminRepository.GetSalaryDisbursementById(salaryDisbursementId);
            if (salaryDisbursement == null)
            {
                return false;
            }
            var client = _clientRepository.GetClientById(salaryDisbursement.Employee.Client.Id);

            var approved = _adminRepository.ApproveSalaryDisbursement(salaryDisbursementId); if (approved)
            {
                if (isBatch)
                {
                    var employeeSalaries = new List<EmployeeDTO>
                    {
                        new EmployeeDTO
                        {
                            FirstName = salaryDisbursement.Employee.FirstName,
                            LastName = salaryDisbursement.Employee.LastName,
                            Salary = salaryDisbursement.Employee.Salary
                }
            };
                    _emailService.SendBatchSalaryDisbursementApprovalEmail(client.Email, employeeSalaries, salaryDisbursement.DisbursementDate.ToString("MMMM yyyy"));

                }
                else
                {
                    // Single employee logic
                    _emailService.SendSalaryDisbursementApprovalEmail(client.Email, new EmployeeDTO
                    {
                        FirstName = salaryDisbursement.Employee.FirstName,
                        LastName = salaryDisbursement.Employee.LastName,
                        Salary = salaryDisbursement.Employee.Salary
                    }, salaryDisbursement.Employee.Salary, salaryDisbursement.DisbursementDate.ToString("MMMM yyyy"));
                }
                return true;
            }
            return false;
        }

        public bool RejectSalaryDisbursement(Guid salaryDisbursementId, bool isBatch = false)
        {
            // return  _adminRepository.RejectSalaryDisbursement(salaryDisbursementId); 
            var salaryDisbursement = _adminRepository.GetSalaryDisbursementById(salaryDisbursementId);
            if (salaryDisbursement == null)
            {
                return false;
            }
            var client = _clientRepository.GetClientById(salaryDisbursement.Employee.Client.Id);

            // Reject the salary disbursement
            var rejected = _adminRepository.RejectSalaryDisbursement(salaryDisbursementId);
            if (rejected)
            {
                if (isBatch)
                {
                    // Send batch rejection email
                    _emailRepository.SendEmailNotification(client.Email, "Salary Disbursement Request Rejected", $"Dear {client.CompanyName}, your salary disbursement request has been rejected in batch.");
                }
                else
                {
                    // Send single rejection email
                    _emailRepository.SendEmailNotification(client.Email, "Salary Disbursement Request Rejected", $"Dear {client.CompanyName}, your salary disbursement request has been rejected.");
                }
            }
            return rejected;


        }



    }
}
