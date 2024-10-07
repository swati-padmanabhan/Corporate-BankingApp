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
    [RoutePrefix("client")]
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
        [Route("")]
        public ActionResult Index()
        {
            ViewBag.Username = User.Identity.Name;
            return View();
        }

        [Route("user-profile")]
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

        [HttpPost]
        [Route("update-balance")]
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
        [Route("manage-beneficiaries")]
        public ActionResult ManageBeneficiaries() => View();

        [Route("get-all-beneficiaries")]
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

        [Route("inbound-beneficiaries")]
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

        [HttpPost]
        [Route("update-beneficiary-status/{id}/{isActive}")]
        public ActionResult UpdateBeneficiaryStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            _beneficiaryService.UpdateBeneficiaryStatus(id, isActive);
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("add-new-beneficiary")]
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

            // Retrieve uploaded files
            var addressProof = Request.Files["uploadedDocs1"];
            var idProof = Request.Files["uploadedDocs2"];

            // Validate Address proof
            if (addressProof != null && addressProof.ContentLength > 0)
            {
                if (!IsValidFile(addressProof))
                {
                    ModelState.AddModelError("BeneficiaryAddressProof", "Address Proof must be an image or PDF and cannot exceed 3MB.");
                }
            }
            else
            {
                ModelState.AddModelError("BeneficiaryAddressProof", "Address Proof is required.");
            }

            // Validate ID proof
            if (idProof != null && idProof.ContentLength > 0)
            {
                if (!IsValidFile(idProof))
                {
                    ModelState.AddModelError("BeneficiaryIdProof", "ID Proof must be an image or PDF and cannot exceed 3MB.");
                }
            }
            else
            {
                ModelState.AddModelError("BeneficiaryIdProof", "ID Proof is required.");
            }

            // If there are any model errors, return them
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            // Proceed to add new beneficiary
            _beneficiaryService.AddNewBeneficiary(beneficiaryDTO, client, new List<HttpPostedFileBase> { addressProof, idProof });
            return Json(new { success = true, message = "Beneficiary added successfully." });
        }

        private bool IsValidFile(HttpPostedFileBase file)
        {
            var validTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
            const int maxSize = 3 * 1024 * 1024; // 3MB
            return validTypes.Contains(file.ContentType) && file.ContentLength <= maxSize;
        }


        [HttpGet]
        [Route("get-beneficiary-by-id/{id}")]
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

        //[HttpPost]
        //[Route("edit-beneficiary")]
        //public ActionResult EditBeneficiary(BeneficiaryDTO beneficiaryDTO)
        //{
        //    if (Session["UserId"] == null)
        //    {
        //        return new HttpStatusCodeResult(401, "Unauthorized");
        //    }

        //    Guid clientId = (Guid)Session["UserId"];
        //    var client = _clientService.GetClientById(clientId);
        //    if (client == null)
        //    {
        //        return new HttpStatusCodeResult(400, "Client not found");
        //    }

        //    var uploadedFiles = new List<HttpPostedFileBase>
        //    {
        //        Request.Files["newIdProof"],
        //        Request.Files["newAddressProof"]
        //    }.Where(file => file != null && file.ContentLength > 0).ToList();

        //    _beneficiaryService.UpdateBeneficiary(beneficiaryDTO, client, uploadedFiles);
        //    return Json(new { success = true, message = "Beneficiary updated successfully" });
        //}

        [HttpPost]
        [Route("edit-beneficiary")]
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

            foreach (var file in uploadedFiles)
            {
                // Validate file size
                if (file.ContentLength > 3 * 1024 * 1024) // 3MB
                {
                    return Json(new { success = false, message = "File size must be less than 3MB." });
                }

                // Validate file type
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!validExtensions.Contains(fileExtension))
                {
                    return Json(new { success = false, message = "Invalid file type. Only images and PDFs are allowed." });
                }
            }

            _beneficiaryService.UpdateBeneficiary(beneficiaryDTO, client, uploadedFiles);
            return Json(new { success = true, message = "Beneficiary updated successfully" });
        }


        public ActionResult ViewAllInboundBeneficiaries()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);
            //for checking onboarding status
            ViewBag.Client = client;
            return View();
        }

        public ActionResult GetAllInboundBeneficiaries()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            var beneficiaries = _beneficiaryService.GetAllInboundBeneficiaries(clientId);
            return Json(beneficiaries, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddInboundBeneficiary(List<Guid> beneficiaryIds)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            Guid clientId = (Guid)Session["UserId"];
            if (beneficiaryIds == null || !beneficiaryIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No beneficiaries selected for addition."
                });
            }
            bool success = true;
            foreach (var id in beneficiaryIds)
            {
                //_clientService.AddInboundBeneficiary(id);   
                _beneficiaryService.AddInboundBeneficiary(clientId, id);
                success = true;
                //break;
            }
            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Inbound Beneficiary/s added successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to add Inbound Beneficiary/s."
                });
            }
        }




        // Manage Employees
        [Route("manage-employees")]
        public ActionResult ManageEmployees() => View();

        [Route("get-all-employees")]
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

        [HttpPost]
        [Route("add-employee")]
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

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
                return Json(new { success = false, message = "Validation failed.", errors }, JsonRequestBehavior.AllowGet);
            }

            if (_clientService.EmailExists(employeeDTO.Email))
            {
                return Json(new { success = false, message = "Email already exists." }, JsonRequestBehavior.AllowGet);
            }

            employeeDTO.IsActive = true;
            _clientService.AddEmployeeDetails(employeeDTO, client);

            return Json(new
            {
                success = true,
                message = "New Employee Added Successfully"
            });
        }

        [HttpGet]
        [Route("get-employee-by-id/{id}")]
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
        [Route("edit-employee")]
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
        [Route("update-employee-status/{id}/{isActive}")]
        public ActionResult UpdateEmployeeStatus(Guid id, bool isActive)
        {
            Guid clientId = (Guid)Session["UserId"];
            _clientService.UpdateEmployeeStatus(id, isActive);
            return Json(new { success = true });
        }

        // Upload CSV
        [HttpPost]
        [Route("upload-csv")]
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
        [Route("process-salary-disbursements")]
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
        [Route("make-payment-requests")]
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
                Amount = 0,  // Default amount
                Beneficiaries = beneficiaryList
            };

            return Json(new { success = true, data = paymentBeneficiaryDTO }, JsonRequestBehavior.AllowGet);
        }


        // Upload Documents
        [Route("upload-documents")]
        public ActionResult UploadDocuments() => View();

        // Generate Reports
        [Route("generate-reports")]
        public ActionResult GenerateReports() => View();
    }
}
