using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Services;
using CorporateBankingApp.Utils;
using Microsoft.AspNetCore.Http;
using NHibernate.Hql.Ast;


namespace CorporateBankingApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(UserDTO userDTO)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return View(userDTO); // Return the view with validation errors
            }

            var loginResult = _userService.LoginActivity(userDTO);
            if (loginResult != null)
            {
                var user = _userService.GetUserByUsername(userDTO.UserName);
                FormsAuthentication.SetAuthCookie(userDTO.UserName, true);
                Session["UserId"] = user.Id;

                if (loginResult == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Client");
            }

            // Add a model error if login fails (optional)
            ModelState.AddModelError("", "Invalid username or password.");
            return View(userDTO);
        }




        [Authorize(Roles = "Admin, Client")]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }




        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(ClientDTO clientDTO)
        {
            if (!ModelState.IsValid)
            {
                // If the model is not valid, return to the view with the current model
                return View(clientDTO);
            }

            // Check if email already exists
            if (_userService.EmailExists(clientDTO.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
            }

            // Check if username already exists
            if (_userService.GetUserByUsername(clientDTO.UserName) != null)
            {
                ModelState.AddModelError("UserName", "Username already exists. Please choose a different one.");
            }

            // Check for duplicate Account Number
            if (_userService.AccountNumberExists(clientDTO.AccountNumber))
            {
                ModelState.AddModelError("AccountNumber", "An account with this account number already exists.");
            }

            // Check for duplicate IFSC code
            if (_userService.IFSCExists(clientDTO.ClientIFSC))
            {
                ModelState.AddModelError("ClientIFSC", "An account with this IFSC code already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            var files = new List<HttpPostedFileBase>();
            var companyIdProof = Request.Files["uploadedFiles1"];
            var addressProof = Request.Files["uploadedFiles2"];

            // Check for Company ID Proof
            if (companyIdProof != null && companyIdProof.ContentLength > 0)
            {
                files.Add(companyIdProof);
            }
            else
            {
                ModelState.AddModelError("Document1", "Company ID Proof is required.");
            }

            // Check for Address Proof
            if (addressProof != null && addressProof.ContentLength > 0)
            {
                files.Add(addressProof);
            }
            else
            {
                ModelState.AddModelError("Document2", "Address Proof is required.");
            }

            // Check if there are any model errors before proceeding
            if (!ModelState.IsValid)
            {
                return View(clientDTO); // Return to view with validation errors
            }

            // Assuming _userService.CreateNewClient can handle the IsActive property
            _userService.CreateNewClient(clientDTO, files);

            return RedirectToAction("RegistrationSuccess");
        }




        public ActionResult RegistrationSuccess()
        {
            return View();
        }

    }
}

