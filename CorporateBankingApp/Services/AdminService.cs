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
                var title = "Client Account Approval Notification";
                var messageContent = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        border-radius: 5px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        padding: 20px;
                    }}
                    .header {{
                        background-color: #4e4187; /* Primary color */
                        color: white;
                        padding: 10px;
                        text-align: center;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        line-height: 1.6;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>{title}</h1>
                    </div>
                    <div class='content'>
                        <p>Dear {client.UserName},</p>
                        <p>We are delighted to inform you that your client account has been successfully approved! All submitted details and documents have met our onboarding requirements.</p>
                        <p>You are now granted full access to our range of services. If you have any questions or need assistance, please do not hesitate to reach out to our support team.</p>
                        <p>Thank you for choosing us!</p>
                    </div>
                </div>
            </body>
            </html>";

                _emailRepository.SendEmailNotification(client.Email, title, messageContent);
            }
            else if (status == "REJECTED")
            {
                client.OnBoardingStatus = CompanyStatus.REJECTED;

                var title = "Client Account Rejection Notification";
                var messageContent = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        border-radius: 5px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        padding: 20px;
                    }}
                    .header {{
                        background-color: #4e4187; /* Primary color */
                        color: white;
                        padding: 10px;
                        text-align: center;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        line-height: 1.6;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>{title}</h1>
                    </div>
                    <div class='content'>
                        <p>Dear {client.UserName},</p>
                        <p>We regret to inform you that your client account application has been declined. This decision was made due to discrepancies found in the details and documents you submitted, which do not meet our onboarding standards.</p>
                        <p>If you have any questions or require clarification regarding this decision, please feel free to contact our support team. We are here to assist you.</p>
                        <p>Thank you for your understanding.</p>
                    </div>
                </div>
            </body>
            </html>";

                _emailRepository.SendEmailNotification(client.Email, title, messageContent);
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

        //outbound client verification
        public List<BeneficiaryDTO> GetBeneficiariesForVerification(UrlHelper urlHelper)
        {
            var beneficiaries = _adminRepository.GetPendingBeneficiaries();
            var beneficiariesDto = beneficiaries.Select(b => new BeneficiaryDTO
            {
                Id = b.Id,
                BeneficiaryName = b.BeneficiaryName,
                AccountNumber = b.AccountNumber,
                BankIFSC = b.BankIFSC,
                ClientName = b.Client.UserName,
                BeneficiaryType = b.BeneficiaryType.ToString().ToUpper(),
                DocumentPaths = b.Documents.Select(d => urlHelper.Content(d.FilePath)).ToList()
            }).ToList();
            return beneficiariesDto;
        }

        public bool UpdateOutboundBeneficiaryOnboardingStatus(Guid id, string status)
        {
            var beneficiary = _adminRepository.GetBeneficiaryById(id);
            if (beneficiary.Client == null)
            {
                // Client not found
                return false;
            }

            // Update onboarding status based on the status string
            if (status == "APPROVED")
            {
                beneficiary.BeneficiaryStatus = CompanyStatus.APPROVED;

                var title = "Beneficiary Approval Notification";
                var messageContent = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        border-radius: 5px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        padding: 20px;
                    }}
                    .header {{
                        background-color: #4e4187; /* Primary color */
                        color: white;
                        padding: 10px;
                        text-align: center;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        line-height: 1.6;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>{title}</h1>
                    </div>
                    <div class='content'>
                        <p>Dear {beneficiary.Client.UserName},</p>
                        <p>We are pleased to inform you that your beneficiary, <strong>{beneficiary.BeneficiaryName}</strong>, has been successfully approved. This decision comes after a thorough verification of the details and documents you submitted.</p>
                        <p>You can now proceed with any transactions involving this beneficiary. Should you have any questions or require further assistance, please do not hesitate to reach out to our support team.</p>
                        <p>Thank you for choosing our services.</p>
                    </div>
                </div>
            </body>
            </html>";

                _emailService.SendClientOnboardingStatusEmail(beneficiary.Client.Email, title, messageContent);
            }
            else if (status == "REJECTED")
            {
                beneficiary.BeneficiaryStatus = CompanyStatus.REJECTED;

                var title = "Beneficiary Application Update";
                var messageContent = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 0;
                        background-color: #f4f4f4;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 20px auto;
                        background-color: #ffffff;
                        border-radius: 5px;
                        box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        padding: 20px;
                    }}
                    .header {{
                        background-color: #4e4187; /* Primary color */
                        color: white;
                        padding: 10px;
                        text-align: center;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        line-height: 1.6;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>{title}</h1>
                    </div>
                    <div class='content'>
                        <p>Dear {beneficiary.Client.UserName},</p>
                        <p>We regret to inform you that the application for your beneficiary, <strong>{beneficiary.BeneficiaryName}</strong>, has been declined. This decision was made due to discrepancies found in the details and documents you submitted.</p>
                        <p>If you need any further assistance or clarification regarding this decision, please feel free to contact our support team. We are here to help you.</p>
                        <p>Thank you for your understanding.</p>
                    </div>
                </div>
            </body>
            </html>";

                _emailService.SendClientOnboardingStatusEmail(beneficiary.Client.Email, title, messageContent);
            }

            _adminRepository.UpdateBeneficiary(beneficiary);
            return true;
        }



        //verify payment
        public IEnumerable<PaymentDTO> GetPendingPaymentsByStatus(CompanyStatus status)
        {
            return _adminRepository.GetPendingPaymentsByStatus(status);
        }

        public void UpdatePaymentStatuses(List<Guid> paymentIds, CompanyStatus status)
        {
            foreach (var paymentId in paymentIds)
            {
                _adminRepository.UpdatePaymentStatus(paymentId, status);

                //fetching the payment details
                var payment = _adminRepository.GetPaymentById(paymentId);

                if (payment != null)
                {
                    var client = _clientRepository.GetClientById(payment.ClientId);
                    if (client != null)
                    {
                        var subject = status == CompanyStatus.APPROVED ? "Payment Approved" : "Payment Rejected";
                        var body = $"Dear {client.UserName}, your payment of {payment.Amount:C} has been {status.ToString().ToLower()}.";
                        _emailService.SendClientOnboardingStatusEmail(client.Email, subject, body);

                    }
                }

            }
        }

        public EmployeeDTO GetEmployeeByDisbursementId(Guid disbursementId)
        {
            var salaryDisbursement = _adminRepository.GetSalaryDisbursementById(disbursementId);

            if (salaryDisbursement == null || salaryDisbursement.Employee == null)
            {
                return null; // Handle this case in ApproveDisbursements
            }

            return new EmployeeDTO
            {
                FirstName = salaryDisbursement.Employee.FirstName,
                LastName = salaryDisbursement.Employee.LastName,
                Email = salaryDisbursement.Employee.Email,
                Salary = salaryDisbursement.Employee.Salary
            };
        }

    }
}