﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CloudinaryDotNet.Actions;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using CorporateBankingApp.Services;
using Newtonsoft.Json;

namespace CorporateBankingApp.Controllers
{
    [RoutePrefix("user")] // Add a route prefix for all actions in this controller
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IClientService _clientService;

        public string RecaptchaToken { get; set; }

        public UserController(IUserService userService, IClientService clientService)
        {
            _userService = userService;
            _clientService = clientService;
        }

        [AllowAnonymous]
        [Route("login")] // Update the route for Login action
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(userDTO);
            }

            string recaptchaToken = Request.Form["g-recaptcha-response"];
            if (string.IsNullOrEmpty(recaptchaToken))
            {
                ModelState.AddModelError("", "reCAPTCHA token is missing.");
                return View(userDTO);
            }

            var isCaptchaValid = ValidateRecaptcha(recaptchaToken);
            if (!isCaptchaValid)
            {
                ModelState.AddModelError("", "reCAPTCHA validation failed.");
                return View(userDTO);
            }

            var loginResult = _userService.LoginActivity(userDTO);
            if (loginResult != null)
            {
                if (loginResult == "Admin")
                {
                    var adminUser = _userService.GetUserByUsername(userDTO.UserName) as Admin;
                    FormsAuthentication.SetAuthCookie(userDTO.UserName, true);
                    Session["UserId"] = adminUser.Id;
                    return RedirectToAction("Index", "Admin");
                }

                var user = _userService.GetUserByUsername(userDTO.UserName) as Client;

                if (user != null)
                {
                    switch (user.OnBoardingStatus)
                    {
                        case CompanyStatus.PENDING:
                            ModelState.AddModelError("", "Your application is still under process. Please wait for approval.");
                            FormsAuthentication.SetAuthCookie(userDTO.UserName, true);
                            Session["UserId"] = user.Id;
                            return View(userDTO); // Do not redirect to the dashboard

                        case CompanyStatus.REJECTED:
                            ModelState.AddModelError("", "Your application has been rejected. Please fill out the form below to reapply.");
                            var clientDTO = new ClientDTO
                            {
                                Id = user.Id,
                                UserName = user.UserName,
                                Email = user.Email,
                                CompanyName = user.CompanyName,
                                Location = user.Location,
                                ContactInformation = user.ContactInformation,
                                AccountNumber = user.AccountNumber,
                                ClientIFSC = user.ClientIFSC,
                                Balance = user.Balance,
                                Documents = user.Documents.Select(d => new DocumentDTO
                                {
                                    DocumentType = d.DocumentType,
                                    FilePath = d.FilePath
                                }).ToList()
                            };
                            FormsAuthentication.SetAuthCookie(userDTO.UserName, true);
                            Session["UserId"] = user.Id;
                            return View("EditClientRegistrationDetails", clientDTO); // Do not redirect to the dashboard

                        case CompanyStatus.APPROVED:
                            FormsAuthentication.SetAuthCookie(userDTO.UserName, true);
                            Session["UserId"] = user.Id;
                            return RedirectToAction("Index", "Client"); // Allow access to the dashboard
                    }
                }
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(userDTO);
        }

        private bool ValidateRecaptcha(string token)
        {
            var secretKey = "6LfQxFgqAAAAAM2yBKJXnU4wZf_c7MBrXJ1Vl0YD";
            var client = new HttpClient();
            var result = client.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}").Result;
            dynamic jsonData = JsonConvert.DeserializeObject(result);
            return jsonData.success;
        }

        [Route("edit-registration")]
        public ActionResult EditClientRegistrationDetails()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);

            // Check if the client's onboarding status is PENDING or REJECTED
            if (client.OnBoardingStatus == CompanyStatus.PENDING || client.OnBoardingStatus == CompanyStatus.REJECTED)
            {
                // Redirect to login or another appropriate action
                return RedirectToAction("Login", "User");
            }

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
        [Route("edit-registration")]
        public ActionResult EditClientRegistrationDetails(ClientDTO clientDTO)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            Guid clientId = (Guid)Session["UserId"];
            var client = _clientService.GetClientById(clientId);

            // Update client details
            client.Email = clientDTO.Email;
            client.CompanyName = clientDTO.CompanyName;
            client.Location = clientDTO.Location;
            client.ContactInformation = clientDTO.ContactInformation;
            client.AccountNumber = clientDTO.AccountNumber;
            client.ClientIFSC = clientDTO.ClientIFSC;
            client.Balance = clientDTO.Balance;

            // Set onboarding status to PENDING for admin approval
            client.OnBoardingStatus = CompanyStatus.PENDING;

            // Handle uploaded files
            var uploadedFiles = new List<HttpPostedFileBase>
    {
        Request.Files["uploadedFiles1"],
        Request.Files["uploadedFiles2"]
    }.Where(file => file != null && file.ContentLength > 0).ToList();

            // Save the edited client registration details
            _clientService.EditClientRegistration(client, uploadedFiles);

            // Redirect to success or login page after edit
            return RedirectToAction("EditRegistrationSuccess"); // You might want to redirect to a success page
        }


        [Route("edit-registration-success")]
        public ActionResult EditRegistrationSuccess()
        {
            return View();
        }




        [Authorize(Roles = "Admin, Client")]
        [Route("logout")] // Update the route for Logout action
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("register")] // Update the route for Register action
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        public ActionResult Register(ClientDTO clientDTO)
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            // Check for existing email, username, account number, and IFSC code
            if (_userService.EmailExists(clientDTO.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
            }
            if (_userService.GetUserByUsername(clientDTO.UserName) != null)
            {
                ModelState.AddModelError("UserName", "Username already exists. Please choose a different one.");
            }
            if (_userService.AccountNumberExists(clientDTO.AccountNumber))
            {
                ModelState.AddModelError("AccountNumber", "An account with this account number already exists.");
            }
            if (_userService.IFSCExists(clientDTO.ClientIFSC))
            {
                ModelState.AddModelError("ClientIFSC", "An account with this IFSC code already exists.");
            }

            // Check if any validation errors were added
            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            // Validate uploaded files
            var files = new List<HttpPostedFileBase>
    {
        Request.Files["uploadedFiles1"],
        Request.Files["uploadedFiles2"]
    };

            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (!IsValidFile(file))
                    {
                        ModelState.AddModelError("Document", "Documents must be images or PDFs and cannot exceed 3MB.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Document", "Documents are required.");
                }
            }

            // Check if any file validation errors were added
            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            // Proceed to create new client
            _userService.CreateNewClient(clientDTO, files);
            return RedirectToAction("RegistrationSuccess");
        }

        private bool IsValidFile(HttpPostedFileBase file)
        {
            var validTypes = new[] { "image/jpeg", "image/png", "image/gif", "application/pdf" };
            const int maxSize = 3 * 1024 * 1024; // 3MB
            return validTypes.Contains(file.ContentType) && file.ContentLength <= maxSize;
        }


        [Route("registration-success")] // Update the route for RegistrationSuccess action
        public ActionResult RegistrationSuccess()
        {
            return View();
        }
    }
}
