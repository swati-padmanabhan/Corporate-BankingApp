using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using CorporateBankingApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApp.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        private readonly IClientService _clientService;

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // GET: Client
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            ViewBag.Username = username;
            return View();
        }
        // GET: ManageBeneficiaries
        public ActionResult ManageBeneficiaries()
        {
            return View();
        }

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




        // GET: MakePaymentRequests
        public ActionResult MakePaymentRequests()
        {
            return View();
        }

        // GET: SalaryDisbursement
        public ActionResult SalaryDisbursement()
        {
            return View();
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