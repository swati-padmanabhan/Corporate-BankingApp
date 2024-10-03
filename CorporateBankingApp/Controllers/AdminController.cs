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
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            ViewBag.Username = username;
            return View();
        }

        [AllowAnonymous]
        public ActionResult AdminRegistration()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult AdminRegistration(AdminDTO adminDTO)
        {
            if (ModelState.IsValid)
            {
                _adminService.RegisterAdmin(adminDTO);
                return RedirectToAction("Login", "User");
            }
            return View(adminDTO);
        }

        public ActionResult ClientApproval()
        {
            var clients = _adminService.GetRegisteredClientsPendingApproval();
            return View(clients);
        }

        public ActionResult ViewClientDetails(Guid id)
        {
            var client = _adminService.GetClientById(id);
            return View(client);
        }

        [HttpPost]
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
        public ActionResult RejectClient(Guid clientId, string rejectionReason)
        {
            // Logic to reject client
            _adminService.RejectClient(clientId);
            var client = _adminService.GetClientById(clientId);

            _emailService.SendOnboardingRejectionEmail(client.Email, rejectionReason);

            return RedirectToAction("ClientApproval");
        }




        // GET: ClientManagement
        public ActionResult ClientManagement()
        {
            return View();
        }

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

        public ActionResult UpdateClientDetails(ClientDTO clientDTO, Guid id)
        {
            _adminService.UpdateClientDetails(clientDTO, id);
            return Json(new { success = true, message = "Client Details Updated Successfully." });
        }

        public ActionResult DeleteClientDetails(Guid id)
        {
            _adminService.DeleteClientDetails(id);
            return Json(new { sucess = true, message = "Client Details Deleted Successfully." });
        }


        //*****************************************************payments*****************************************************
        // GET: PaymentApprovals
        public ActionResult PaymentApprovals()
        {
            var pendingPayments = _adminService.GetPendingPaymentsByStatus(CompanyStatus.PENDING);
            return View(pendingPayments);
        }

        [HttpPost]
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
        public ActionResult BeneficiaryManagement()
        {
            return View();
        }

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
        public ActionResult SalaryDisbursementApprovals()
        {
            var pendingDisbursements = _adminService.ListPendingSalaryDisbursements();
            return View(pendingDisbursements);
        }


        [HttpPost]
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
                if (!approved)
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
                    message = "Salary disbursements approved successfully."
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
        public ActionResult ReportGeneration()
        {
            return View();
        }
    }
}