using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApp.Controllers
{
    public class ReportController : Controller
    {
        public ActionResult EmployeeReport()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var query = from e in session.Query<Employee>()
                            from sd in e.SalaryDisbursements.DefaultIfEmpty() // Left join to get employees with or without salary disbursements
                            select new EmployeeReportDTO
                            {
                                EmployeeId = e.Id,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                Email = e.Email,
                                Designation = e.Designation,
                                Salary = e.Salary,
                                DisbursementDate = sd != null ? (DateTime?)sd.DisbursementDate : null,
                                SalaryStatus = sd != null ? (CompanyStatus?)sd.SalaryStatus : null
                            };

                var employeeReports = query.ToList();
                return View(employeeReports);
            }
        }


        public ActionResult BeneficiaryReport()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var beneficiaries = session.Query<Beneficiary>()
                                           .FetchMany(b => b.Payments)
                                           .ToList();

                var viewModel = beneficiaries.SelectMany(b => b.Payments.Select(p => new BeneficiaryReportDTO
                {
                    BeneficiaryId = b.Id,
                    BeneficiaryName = b.BeneficiaryName,
                    AccountNumber = b.AccountNumber,
                    BankIFSC = b.BankIFSC,
                    BeneficiaryStatus = b.BeneficiaryStatus,
                    BeneficiaryType = b.BeneficiaryType,


                    // Payment details
                    Amount = p.Amount,
                    PaymentRequestDate = p.PaymentRequestDate,
                    PaymentApprovalDate = p.PaymentApprovalDate,
                    PaymentStatus = p.PaymentStatus
                })).ToList();

                return View(viewModel);
            }
        }


        public ActionResult ClientList()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var clients = session.Query<Client>()
                                     .Select(c => new ClientReportDTO
                                     {
                                         Id = c.Id,
                                         CompanyName = c.CompanyName // Assuming your Client model has a Name property
                                     }).ToList();

                return View(clients);
            }
        }


        public ActionResult EmployeeReportByClient(Guid clientId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var query = from e in session.Query<Employee>()
                            where e.Client.Id == clientId
                            from sd in e.SalaryDisbursements.DefaultIfEmpty()
                            select new EmployeeReportDTO
                            {
                                EmployeeId = e.Id,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                Email = e.Email,
                                Designation = e.Designation,
                                Salary = e.Salary,
                                DisbursementDate = sd != null ? (DateTime?)sd.DisbursementDate : null,
                                SalaryStatus = sd != null ? (CompanyStatus?)sd.SalaryStatus : null
                            };

                var employeeReports = query.ToList();
                return View(employeeReports);
            }
        }

        public ActionResult BeneficiaryReportByClient(Guid clientId)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var query = from b in session.Query<Beneficiary>()
                            where b.Client.Id == clientId
                            from p in b.Payments.DefaultIfEmpty() // To handle beneficiaries with no payments
                            select new BeneficiaryReportDTO
                            {
                                BeneficiaryId = b.Id,
                                BeneficiaryName = b.BeneficiaryName,
                                AccountNumber = b.AccountNumber,
                                BankIFSC = b.BankIFSC,
                                BeneficiaryStatus = b.BeneficiaryStatus,
                                BeneficiaryType = b.BeneficiaryType,

                                // Payment details
                                Amount = p != null ? (double?)p.Amount : null, // Amount paid to beneficiary
                                PaymentRequestDate = p != null ? (DateTime?)p.PaymentRequestDate : null,
                                PaymentApprovalDate = p != null ? (DateTime?)p.PaymentApprovalDate : null,
                                PaymentStatus = p != null ? (CompanyStatus?)p.PaymentStatus : null
                            };

                var beneficiaryReports = query.ToList();
                return View(beneficiaryReports);
            }
        }

    }
}