using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Repositories;
using CorporateBankingApp.Services;
using NHibernate.Criterion;
using Razorpay.Api;

namespace CorporateBankingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IEmailService _emailService;
        private readonly IClientRepository _clientRepository;

        public AdminController(IAdminService adminService, IEmailService emailService, IClientRepository clientRepository)
        {
            _adminService = adminService;
            _emailService = emailService;
            _clientRepository = clientRepository;
        }

        // GET: Admin
        [Route("")]
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            ViewBag.Username = username;
            return View();
        }

        [AllowAnonymous]
        [Route("registration")]
        public ActionResult AdminRegistration()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("registration")]
        public ActionResult AdminRegistration(AdminDTO adminDTO)
        {
            if (ModelState.IsValid)
            {
                _adminService.RegisterAdmin(adminDTO);
                return RedirectToAction("Login", "User");
            }
            return View(adminDTO);
        }

        [Route("client-approval")]
        public ActionResult ClientApproval(int page = 1, int pageSize = 2)
        {

            var urlHelper = new UrlHelper(Request.RequestContext); // Create UrlHelper here
            var clients = _adminService.GetClientsForVerification(urlHelper);

            var totalRecords = clients.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Skip and take to paginate the list
            var pagedClients = clients.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(pagedClients);
        }


        [Route("client-details/{id}")]
        public ActionResult ViewClientDetails(Guid id)
        {
            var client = _adminService.GetClientById(id);
            return View(client);
        }

        [HttpPost]
        [Route("approve-client")]
        // Action for accepting client
        public ActionResult ApproveClient(Guid clientId)
        {
            _adminService.ApproveClient(clientId);
            var client = _adminService.GetClientById(clientId);
            _emailService.SendOnboardingAcceptanceEmail(client.Email);

            return RedirectToAction("ClientApproval");
        }

        // Action for rejecting client
        [HttpPost]
        [Route("reject-client")]
        public ActionResult RejectClient(Guid clientId, string rejectionReason)
        {
            // Logic to reject client
            _adminService.RejectClient(clientId);
            var client = _adminService.GetClientById(clientId);

            _emailService.SendOnboardingRejectionEmail(client.Email, rejectionReason);

            return RedirectToAction("ClientApproval");
        }


        // GET: ClientManagement
        [Route("client-management")]
        public ActionResult ClientManagement()
        {
            return View();
        }

        [Route("get-all-clients")]
        public ActionResult GetAllClients(int page, int rows, string sidx, string sord, bool _search, string searchField, string searchString, string searchOper)
        {

            var clients = _adminService.GetAllClients();
            var clientList = clients;

            //check if search operation was requested
            if (_search && searchField == "CompanyName" && searchOper == "eq")
            {
                clientList = clients.Where(cd => cd.CompanyName == searchString).ToList();
            }

            //Get total count of records(for pagination)
            int totalCount = clients.Count();
            //Calculate total pages
            int totalPages = (int)Math.Ceiling((double)totalCount / rows);

            //for sorting sort acc to username,email,companyname,contactinfo,location,balance,onboarding status
            switch (sidx)
            {
                case "UserName":
                    clientList = sord == "asc" ? clientList.OrderBy(cl => cl.UserName).ToList()
                        : clientList.OrderByDescending(cd => cd.UserName).ToList();
                    break;
                case "Email":
                    clientList = sord == "asc" ? clientList.OrderBy(cl => cl.Email).ToList()
                        : clientList.OrderByDescending(cd => cd.Email).ToList();
                    break;
                case "CompanyName":
                    clientList = sord == "asc" ? clientList.OrderBy(cl => cl.CompanyName).ToList()
                        : clientList.OrderByDescending(cd => cd.CompanyName).ToList();
                    break;
                case "Location":
                    clientList = sord == "asc" ? clientList.OrderBy(cl => cl.Location).ToList()
                        : clientList.OrderByDescending(cd => cd.Location).ToList();
                    break;
                case "ContactInformation":
                    clientList = sord == "asc" ? clientList.OrderBy(cl => cl.ContactInformation).ToList()
                        : clientList.OrderByDescending(cd => cd.ContactInformation).ToList();
                    break;

                case "Balance":
                    clientList = sord == "asc" ? clientList.OrderBy(cl => cl.Balance).ToList()
                        : clientList.OrderByDescending(cd => cd.Balance).ToList();
                    break;
                case "OnBoardingStatus":
                    clientList = sord == "asc" ? clientList.OrderBy(cl => cl.OnBoardingStatus).ToList()
                        : clientList.OrderByDescending(cd => cd.OnBoardingStatus).ToList();
                    break;
            }


            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalCount,
                rows = clientList.Select(cl => new
                {
                    cell = new string[]
                    {
                 cl.Id.ToString(), //change column names
                 cl.UserName,
                 cl.Email,
                 cl.CompanyName,
                 cl.Location,
                 cl.ContactInformation,
                 cl.Balance.ToString(),
                 cl.OnBoardingStatus.ToString()
                    }
                }).Skip(page - 1 * rows).Take(rows).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        [Route("update-client-details/{id}")]
        public ActionResult UpdateClientDetails(ClientDTO clientDTO, Guid id)
        {
            _adminService.UpdateClientDetails(clientDTO, id);
            return Json(new { success = true, message = "Client Details Updated Successfully." });
        }

        [Route("delete-client-details/{id}")]
        public ActionResult DeleteClientDetails(Guid id)
        {
            _adminService.DeleteClientDetails(id);
            return Json(new { sucess = true, message = "Client Details Deleted Successfully." });
        }


        //*****************************************************payments*****************************************************
        [Route("payment-approvals")]
        public ActionResult PaymentApprovals(int page = 1, int pageSize = 10)
        {
            var pendingPayments = _adminService.GetPendingPaymentsByStatus(CompanyStatus.PENDING);

            var totalRecords = pendingPayments.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // Skip and take for pagination
            var pagedPayments = pendingPayments.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(pagedPayments);
        }


        [HttpPost]
        [Route("approve-payments")]
        public JsonResult ApprovePayments(List<Guid> disbursementIds)
        {
            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new { success = false, message = "No payments selected for approval." });
            }

            try
            {
                _adminService.UpdatePaymentStatuses(disbursementIds, CompanyStatus.APPROVED);
                return Json(new { success = true, message = "Payments approved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while approving payments: {ex.Message}" });
            }
        }

        // Reject payments
        [HttpPost]
        [Route("reject-payments")]
        public JsonResult RejectPayments(List<Guid> disbursementIds)
        {
            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new { success = false, message = "No payments selected for rejection." });
            }

            try
            {
                _adminService.UpdatePaymentStatuses(disbursementIds, CompanyStatus.REJECTED);
                return Json(new { success = true, message = "Payments rejected successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error while rejecting payments: {ex.Message}" });
            }



        }


        //*****************************************verify outbound clients*****************************************
        // GET: BeneficiaryManagement
        [Route("beneficiary-management")]
        public ActionResult BeneficiaryManagement()
        {
            return View();
        }

        [Route("get-beneficiary-for-verification")]
        public ActionResult GetOutboundBeneficiaryForVerification()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var urlHelper = new UrlHelper(Request.RequestContext); // Create UrlHelper here
            var beneficiaryDtos = _adminService.GetBeneficiariesForVerification(urlHelper);
            return Json(beneficiaryDtos, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("update-beneficiary-status/{id:guid}/{status}")]
        public ActionResult UpdateOutboundBeneficiaryOnboardingStatus(Guid id, string status)
        {
            var result = _adminService.UpdateOutboundBeneficiaryOnboardingStatus(id, status);
            if (result)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Failed to update status" });
            }
        }


        // GET: SalaryDisbursementApprovals
        [Route("salary-disbursement")]
        public ActionResult SalaryDisbursementApprovals(int page = 1, int pageSize = 5)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                // First, filter only the pending salary disbursements
                var query = from sd in session.Query<SalaryDisbursement>()
                            where sd.SalaryStatus == CompanyStatus.PENDING
                            select new SalaryDisbursementDTO
                            {
                                SalaryDisbursementId = sd.Id,
                                CompanyName = sd.Employee.Client.UserName,
                                EmployeeFirstName = sd.Employee.FirstName,
                                EmployeeLastName = sd.Employee.LastName,
                                Salary = sd.Employee.Salary,
                                SalaryStatus = sd.SalaryStatus,
                                DisbursementDate = sd.DisbursementDate
                            };

                // Count only the "Pending" records
                var totalRecords = query.Count();

                // Apply pagination after filtering
                var salaryDisbursements = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Calculate total pages based on the filtered records
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                // Set pagination details in ViewBag
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                return View(salaryDisbursements);
            }
        }







        [HttpPost]
        [Route("approve-disbursements")]
        public ActionResult ApproveDisbursements(List<Guid> disbursementIds)
        {
            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No salary disbursement selected for approval."
                });
            }

            bool success = true;

            foreach (var id in disbursementIds)
            {
                bool approved = _adminService.ApproveSalaryDisbursement(id, true);

                if (approved)
                {
                    var employee = _adminService.GetEmployeeByDisbursementId(id);
                    if (employee != null)
                    {
                        var payslip = new PayslipDTO
                        {
                            EmployeeName = $"{employee.FirstName} {employee.LastName}",
                            Salary = employee.Salary,
                            Month = DateTime.Now.ToString("MMMM"),
                            //CompanyName = $"{}" // Set the company name accordingly
                        };

                        _emailService.SendPayslipEmail(employee.Email, payslip);
                    }
                }
                else
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Salary disbursements approved and emails sent successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to approve salary disbursements."
                });
            }
        }


        [HttpPost]
        [Route("reject-disbursements")]
        public ActionResult RejectDisbursements(List<Guid> disbursementIds)
        {

            if (disbursementIds == null || !disbursementIds.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No salary disbursement selected for rejection."
                });
            }

            bool success = true;
            foreach (var id in disbursementIds)
            {
                bool rejected = _adminService.RejectSalaryDisbursement(id, true);
                if (!rejected)
                {
                    success = false;
                    break;
                }
            }

            if (success)
            {
                return Json(new
                {
                    success = true,
                    message = "Salary disbursements rejected successfully."
                });
            }
            else
            {
                return Json(new
                {
                    success = false,
                    message = "Failed to reject salary disbursements."
                });
            }
        }


        // GET: ReportGeneration
        [Route("report-generation")]
        public ActionResult ReportGeneration()
        {
            return View();
        }
    }
}