﻿using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Services;
using CsvHelper;
using dotenv.net.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly IPaymentService _paymentService;

        public ClientController(IClientService clientService, IBeneficiaryService beneficiaryService, IPaymentService paymentService)
        {
            _clientService = clientService;
            _beneficiaryService = beneficiaryService;
            _paymentService = paymentService;
        }

        // GET: Client
        public ActionResult Index()
        {
            ViewBag.Username = User.Identity.Name;
            return View();
        }

        public ActionResult UserProfile()
        {
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);

            if (client == null)
            {
                return HttpNotFound();
            }

            var clientDto = new ClientDTO
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
            };

            return View(clientDto);
        }

        // Edit Client Registration Details
        public ActionResult EditClientRegistrationDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            var clientDTO = new ClientDTO
            {
                Email = client.Email,
                CompanyName = client.CompanyName,
                Location = client.Location,
                ContactInformation = client.ContactInformation,
                AccountNumber = client.AccountNumber,
                ClientIFSC = client.ClientIFSC,
                Balance = client.Balance,
                Documents = client.Documents.Select(d => new DocumentDTO
                {
                    DocumentType = d.DocumentType,
                    FilePath = d.FilePath,
                    UploadDate = d.UploadDate
                }).ToList()
            };
            return View(clientDTO);
        }

        [HttpPost]
        public ActionResult EditClientRegistrationDetails(ClientDTO clientDTO)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            client.Email = clientDTO.Email;
            client.CompanyName = clientDTO.CompanyName;
            client.Location = clientDTO.Location;
            client.ContactInformation = clientDTO.ContactInformation;
            client.AccountNumber = clientDTO.AccountNumber;
            client.ClientIFSC = clientDTO.ClientIFSC;
            client.Balance = clientDTO.Balance;
            client.OnBoardingStatus = CompanyStatus.PENDING;

            var uploadedFiles = new List<HttpPostedFileBase>
            {
                Request.Files["uploadedFiles1"],
                Request.Files["uploadedFiles2"]
            }.Where(file => file != null && file.ContentLength > 0).ToList();

            _clientService.EditClientRegistration(client, uploadedFiles);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateBalance(double newBalance)
        {
            Guid clientId = (Guid)Session["UserId"];

            if (newBalance < 0)
            {
                ModelState.AddModelError("Balance", "Balance cannot be negative.");
                return RedirectToAction("UserProfile");
            }

            _clientService.UpdateClientBalance(clientId, newBalance);
            TempData["SuccessMessage"] = "Balance updated successfully!";
            return RedirectToAction("UserProfile");
        }

        // Manage Beneficiaries
        public ActionResult ManageBeneficiaries() => View();

        public ActionResult GetAllOutboundBeneficiaries()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var urlHelper = new UrlHelper(Request.RequestContext);
            var beneficiaries = _beneficiaryService.GetAllOutboundBeneficiaries(clientId, urlHelper);
            return Json(beneficiaries, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InboundBeneficiaries()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var inboundBeneficiaries = session.Query<Beneficiary>()
                    .Where(b => b.BeneficiaryType == BeneficiaryType.INBOUND)
                    .ToList();

                return View(inboundBeneficiaries);
            }
        }

        public ActionResult UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            _beneficiaryService.UpdateBeneficiaryStatus(id, isActive);
            return Json(new { success = true });
        }

        public ActionResult AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            if (client == null)
            {
                return new HttpStatusCodeResult(400, "Client not found");
            }

            var uploadedFiles = new List<HttpPostedFileBase>
            {
                Request.Files["uploadedDocs1"],
                Request.Files["uploadedDocs2"]
            }.Where(file => file != null && file.ContentLength > 0).ToList();

            _beneficiaryService.AddNewBeneficiary(beneficiaryDTO, client, uploadedFiles);
            return Json(new { success = true });
        }

        public ActionResult GetBeneficiaryById(Guid id)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            var beneficiary = _beneficiaryService.GetBeneficiaryById(id);
            if (beneficiary == null)
            {
                return Json(new { success = false, message = "Beneficiary not found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                beneficiary = new
                {
                    beneficiary.Id,
                    beneficiary.BeneficiaryName,
                    beneficiary.AccountNumber,
                    beneficiary.BankIFSC
                }
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditBeneficiary(BeneficiaryDTO beneficiaryDTO)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            if (client == null)
            {
                return new HttpStatusCodeResult(400, "Client not found");
            }

            var uploadedFiles = new List<HttpPostedFileBase>
            {
                Request.Files["newIdProof"],
                Request.Files["newAddressProof"]
            }.Where(file => file != null && file.ContentLength > 0).ToList();

            _beneficiaryService.UpdateBeneficiary(beneficiaryDTO, client, uploadedFiles);
            return Json(new { success = true, message = "Beneficiary updated successfully" });
        }

        // Manage Employees
        public ActionResult ManageEmployees() => View();

        public ActionResult GetAllEmployees()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var employees = _clientService.GetAllEmployees(clientId);

            var employeeDtos = employees.Select(e => new EmployeeDTO
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Phone = e.Phone,
                Designation = e.Designation,
                Salary = e.Salary,
                IsActive = e.IsActive
            }).ToList();

            return Json(employeeDtos, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Add(EmployeeDTO employeeDTO)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            if (client == null)
            {
                return new HttpStatusCodeResult(400, "Client not found");
            }

            employeeDTO.IsActive = true;
            _clientService.AddEmployeeDetails(employeeDTO, client);

            return Json(new
            {
                employeeDTO.Id,
                employeeDTO.FirstName,
                employeeDTO.LastName,
                employeeDTO.Email,
                employeeDTO.Phone,
                employeeDTO.Designation,
                employeeDTO.Salary,
                employeeDTO.IsActive
            });
        }

        public ActionResult GetEmployeeById(Guid id)
        {
            var employee = _clientService.GetEmployeeById(id);
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee Not Found" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                employee = new
                {
                    employee.Id,
                    employee.FirstName,
                    employee.LastName,
                    employee.Email,
                    employee.Phone,
                    employee.Designation,
                    employee.Salary,
                }
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeDTO employeeDTO)
        {
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            if (client == null)
            {
                return new HttpStatusCodeResult(400, "Client not found");
            }

            _clientService.UpdateEmployeeDetails(employeeDTO, client);
            return Json(new { success = true, message = "Employee updated successfully" });
        }

        [HttpPost]
        public ActionResult UpdateEmployeeStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            _clientService.UpdateEmployeeStatus(id, isActive);
            return Json(new { success = true });
        }

        // Upload CSV
        [HttpPost]
        public ActionResult UploadCsv()
        {
            var csvFile = Request.Files["csvFile"];
            if (Session["UserId"] == null)
            {
                return new HttpStatusCodeResult(401, "Unauthorized");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            if (client == null)
            {
                return new HttpStatusCodeResult(400, "Client not found");
            }

            if (csvFile != null && csvFile.ContentLength > 0)
            {
                try
                {
                    _clientService.UploadEmployeeCsv(csvFile, client);
                    return RedirectToAction("ManageEmployees");
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }
            return Json(new { success = false, message = "No file uploaded." });
        }

        // Salary Disbursement
        [HttpPost]
        public ActionResult ProcessSalaryDisbursements(List<Guid> employeeIds, bool isBatch)
        {
            if (employeeIds == null || !employeeIds.Any())
            {
                return Json(new { success = false, message = "No employees have been selected for salary disbursement." });
            }

            try
            {
                var result = _clientService.ProcessSalaryDisbursements(employeeIds, isBatch, out var excludedEmployees);
                if (result)
                {
                    if (isBatch)
                    {
                        return Json(new
                        {
                            success = true,
                            message = excludedEmployees.Any() ?
                            "Salary disbursement has started, but some employees were excluded as they already received their salary this month." :
                            "Salary disbursement has been initiated for all employees."
                        });
                    }
                    return Json(new
                    {
                        success = true,
                        message = excludedEmployees.Any() ?
                        "Salary has already been distributed to these employees for this month." :
                        "Salary disbursement has been initiated."
                    });
                }
                return Json(new { success = false, message = "An error occurred during the salary disbursement process." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        // Payments
        public ActionResult MakePaymentRequests()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var beneficiaryList = _clientService.GetBeneficiaryList(clientId);

            if (beneficiaryList == null || !beneficiaryList.Any())
            {
                return Json(new { success = false, message = "No beneficiaries found" }, JsonRequestBehavior.AllowGet);
            }

            var model = new BeneficiaryPaymentDTO
            {
                Amount = 0,
                Beneficiaries = beneficiaryList
            };
            return View(model);
        }

        [HttpGet]
        public ActionResult GetBeneficiaryListForPayment()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var beneficiaryList = _clientService.GetBeneficiaryList(clientId);

            if (beneficiaryList == null || !beneficiaryList.Any())
            {
                return Json(new { success = false, message = "No beneficiaries found" }, JsonRequestBehavior.AllowGet);
            }

            var paymentBeneficiaryDTO = new BeneficiaryPaymentDTO
            {
                Amount = 0,
                Beneficiaries = beneficiaryList
            };

            return Json(new { success = true, data = paymentBeneficiaryDTO }, JsonRequestBehavior.AllowGet);
        }

        // Upload Documents
        public ActionResult UploadDocuments() => View();

        // Generate Reports
        public ActionResult GenerateReports() => View();
    }
}
