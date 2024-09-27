using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Services;
using Microsoft.AspNetCore.Http;


namespace CorporateBankingApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;


        public UserController(IUserService userService, ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        //public ActionResult Login(UserDTO userDTO)
        //{
        //    var loginResult = _userService.LoginActivity(userDTO);
        //    if(loginResult != null)
        //    {
        //        if(loginResult == "Admin")
        //        {
        //            return RedirectToAction("Index", "Admin");
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Client");
        //        }
        //    }
        //    return View(userDTO);
        //}

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
                else
                {
                    return RedirectToAction("Index", "Client");
                }
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

        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult Register(ClientDTO clientDTO)
        //{
        //    var uploadedFiles = new List<HttpPostedFileBase>();

        //    var companyIdProof = Request.Files["uploadedFiles1"];
        //    var addressProof = Request.Files["uploadedFiles2"];

        //    if (companyIdProof != null && companyIdProof.ContentLength > 0)
        //    {
        //        uploadedFiles.Add(companyIdProof);
        //    }

        //    if (addressProof != null && addressProof.ContentLength > 0)
        //    {
        //        uploadedFiles.Add(addressProof);
        //    }
        //    _userService.CreateNewClient(clientDTO, uploadedFiles);
        //    return RedirectToAction("Login");



        //}

        [HttpPost]
        public ActionResult UploadDocument(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
            {
                var documentUrl = _cloudinaryService.UploadDocument(file);

                var document = new Document
                {
                    DocumentName = file.FileName,
                    DocumentLink = documentUrl,
                    // Assign Client later during registration
                };

                return Json(new
                {
                    success = true,
                    documentName = file.FileName,
                    documentLink = documentUrl
                });
            }

            return Json(new { success = false });
        }






    }
}

