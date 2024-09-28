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
            var files = new List<HttpPostedFileBase>();

            var companyIdProof = Request.Files["uploadedFiles1"];
            var addressProof = Request.Files["uploadedFiles2"];

            if (companyIdProof != null && companyIdProof.ContentLength > 0)
            {
                files.Add(companyIdProof);
            }

            if (addressProof != null && addressProof.ContentLength > 0)
            {
                files.Add(addressProof);
            }
            _userService.CreateNewClient(clientDTO, files);
            return RedirectToAction("Login");
        }

    }
}

