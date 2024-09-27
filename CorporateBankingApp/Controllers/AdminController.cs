﻿using System;
using System.Linq;
using System.Web.Mvc;
using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Services;

namespace CorporateBankingApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        // GET: Admin
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            ViewBag.Username = username;
            return View();
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
        public ActionResult ApproveClient(Guid id)
        {
            _adminService.ApproveClient(id);
            return RedirectToAction("ClientApproval");
        }

        [HttpPost]
        public ActionResult RejectClient(Guid id)
        {
            _adminService.RejectClient(id);
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



        // GET: PaymentApprovals
        public ActionResult PaymentApprovals()
        {
            return View();
        }

        // GET: BeneficiaryManagement
        public ActionResult BeneficiaryManagement()
        {
            return View();
        }

        // GET: SalaryDisbursementApprovals
        public ActionResult SalaryDisbursementApprovals()
        {
            return View();
        }

        // GET: ReportGeneration
        public ActionResult ReportGeneration()
        {
            return View();
        }
    }
}