using CorporateBankingApp.Data;
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

        public ClientController(IClientService clientService, IBeneficiaryService beneficiaryService)
        {
            _clientService = clientService;
            _beneficiaryService = beneficiaryService;
        }

        // GET: Client
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            ViewBag.Username = username;
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

        //*******************************************Client reupload documents*******************************************
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
                //UserName = client.UserName,
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
            var uploadedFiles = new List<HttpPostedFileBase>();

            var companyIdProof = Request.Files["uploadedFiles1"];
            var addressProof = Request.Files["uploadedFiles2"];

            if (companyIdProof != null && companyIdProof.ContentLength > 0)
            {
                uploadedFiles.Add(companyIdProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }

            _clientService.EditClientRegistration(client, uploadedFiles);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult UpdateBalance(double newBalance)
        {
            Guid clientId = (Guid)Session["UserId"];

            // Validate the new balance, if necessary
            if (newBalance < 0)
            {
                ModelState.AddModelError("Balance", "Balance cannot be negative.");
                return RedirectToAction("UserProfile");
            }

            _clientService.UpdateClientBalance(clientId, newBalance);
            TempData["SuccessMessage"] = "Balance updated successfully!";
            return RedirectToAction("UserProfile");
        }


        // GET: ManageBeneficiaries
        public ActionResult ManageBeneficiaries()
        {
            return View();
        }

        public ActionResult GetAllOutboundBeneficiaries()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var urlHelper = new UrlHelper(Request.RequestContext); // Create UrlHelper here
            var beneficiaries = _beneficiaryService.GetAllOutboundBeneficiaries(clientId, urlHelper);
            return Json(beneficiaries, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
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
            var uploadedFiles = new List<HttpPostedFileBase>();

            var idProof = Request.Files["uploadedDocs1"];
            var addressProof = Request.Files["uploadedDocs1"];


            if (idProof != null && idProof.ContentLength > 0)
            {
                uploadedFiles.Add(idProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }
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
            var existingBeneficiary = _beneficiaryService.GetBeneficiaryById(beneficiaryDTO.Id);
            var uploadedFiles = new List<HttpPostedFileBase>();

            var idProof = Request.Files["newIdProof"];
            var addressProof = Request.Files["newAddressProof"];


            if (idProof != null && idProof.ContentLength > 0)
            {
                uploadedFiles.Add(idProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                uploadedFiles.Add(addressProof);
            }
            _beneficiaryService.UpdateBeneficiary(beneficiaryDTO, client, uploadedFiles);
            return Json(new { success = true, message = "Beneficiary updated successfully" });
        }



        //******************************************Employee Management******************************************

        // GET: ManageEmployees
        public ActionResult ManageEmployees()
        {
            return View();
        }

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



        public ActionResult Add(EmployeeDTO employeeDTO) //employeedto
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
            //employeedto.Client = client;

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
            // employeeDto.Client = client;
            _clientService.UpdateEmployeeDetails(employeeDTO, client);

            return Json(new { success = true, message = "Employee updated successfully" });
        }


        [HttpPost]
        public ActionResult UpdateEmployeeStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            _clientService.UpdateEmployeeStatus(id, isActive);
            return Json(new { success = true });
        }


        //************************************Employee CSV************************************
        [HttpPost]
        public ActionResult UploadCsv()
        {
            var file = new List<HttpPostedFileBase>();

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
                file.Add(csvFile);
            }

            try
            {
                _clientService.UploadEmployeeCsv(csvFile, client);
                //return Json(new { success = true, message = "CSV uploaded and employees added successfully." });
                return RedirectToAction("ManageEmployees");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



        // GET: MakePaymentRequests
        public ActionResult MakePaymentRequests()
        {
            // Retrieve beneficiaries stored in session
            List<Beneficiary> paymentBeneficiaries = Session["PaymentBeneficiaries"] as List<Beneficiary> ?? new List<Beneficiary>();

            return View(paymentBeneficiaries);
        }

        //******************************************Salary Disbursement******************************************

        // GET: SalaryDisbursement
        //public ActionResult SalaryDisbursement()
        //{
        //    return View();
        //}

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
                        // Message for batch processing
                        if (excludedEmployees.Any())
                        {
                            return Json(new { success = true, message = "Salary disbursement has started, but some employees were excluded as they already received their salary this month." });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Salary disbursement has been initiated for all employees." });
                        }
                    }
                    else
                    {
                        if (excludedEmployees.Any())
                        {
                            return Json(new { success = false, message = "Salary has already been distributed to these employees for this month." });
                        }
                        else
                        {
                            return Json(new { success = true, message = "Salary disbursement has been initiated." });
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "An error occurred during the salary disbursement process." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred: " + ex.Message });
            }
        }

        // GET: UploadDocuments
        public ActionResult UploadDocuments()
        {
            return View();
        }

        // GET: GenerateReports
        public ActionResult GenerateReports()
        {
            return View();
        }
    }
}