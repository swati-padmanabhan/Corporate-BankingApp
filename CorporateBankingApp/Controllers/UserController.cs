using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CloudinaryDotNet.Actions;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Services;
using Newtonsoft.Json;

namespace CorporateBankingApp.Controllers
{
    [RoutePrefix("user")] // Add a route prefix for all actions in this controller
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public string RecaptchaToken { get; set; }

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [Route("login")] // Update the route for Login action
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")] // Update the route for Login POST action
        public ActionResult Login(UserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(userDTO);
            }

            // Retrieve the reCAPTCHA token from the request
            string recaptchaToken = Request.Form["g-recaptcha-response"];

            // Check if the reCAPTCHA token is null or empty
            if (string.IsNullOrEmpty(recaptchaToken))
            {
                ModelState.AddModelError("", "reCAPTCHA token is missing.");
                return View(userDTO);
            }

            // Validate the reCAPTCHA token with your secret key
            var isCaptchaValid = ValidateRecaptcha(recaptchaToken);
            if (!isCaptchaValid)
            {
                ModelState.AddModelError("", "reCAPTCHA validation failed.");
                return View(userDTO);
            }

            var loginResult = _userService.LoginActivity(userDTO);
            if (loginResult != null)
            {
                var user = _userService.GetUserByUsername(userDTO.UserName);
                FormsAuthentication.SetAuthCookie(userDTO.UserName, true);
                Session["UserId"] = user.Id;

                return loginResult == "Admin" ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Client");
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
        [Route("register")] // Update the route for Register POST action
        public ActionResult Register(ClientDTO clientDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(clientDTO);
            }

            // Check for email, username, account number, and IFSC existence
            if (_userService.EmailExists(clientDTO.Email))
                ModelState.AddModelError("Email", "An account with this email already exists.");
            if (_userService.GetUserByUsername(clientDTO.UserName) != null)
                ModelState.AddModelError("UserName", "Username already exists. Please choose a different one.");
            if (_userService.AccountNumberExists(clientDTO.AccountNumber))
                ModelState.AddModelError("AccountNumber", "An account with this account number already exists.");
            if (_userService.IFSCExists(clientDTO.ClientIFSC))
                ModelState.AddModelError("ClientIFSC", "An account with this IFSC code already exists.");

            if (!ModelState.IsValid) return View(clientDTO);

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
                        ModelState.AddModelError("Document", "Document must be an image or PDF and cannot exceed 5MB.");
                    }
                }
                else
                {
                    ModelState.AddModelError("Document", "Document is required.");
                }
            }

            if (!ModelState.IsValid) return View(clientDTO);

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
