using CorporateBankingApp.Data;
using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApp.Controllers
{
    [RoutePrefix("report")] // Route prefix for all actions in this controller
    public class ReportController : Controller
    {
        // Route: GET /report/employee-report
        [Route("employee")]
        public ActionResult EmployeeReport(string searchEmail = "", DateTime? fromDate = null, DateTime? toDate = null, int page = 1, int pageSize = 10)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var query = from e in session.Query<Employee>()
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

                // Apply the search by email if provided
                if (!string.IsNullOrWhiteSpace(searchEmail))
                {
                    query = query.Where(e => e.Email.ToLower().Contains(searchEmail.ToLower()));
                }

                // Apply the date range filter if both from and to dates are provided
                if (fromDate.HasValue && toDate.HasValue)
                {
                    query = query.Where(e => e.DisbursementDate.HasValue &&
                                             e.DisbursementDate.Value >= fromDate.Value &&
                                             e.DisbursementDate.Value <= toDate.Value);
                }
                else if (fromDate.HasValue) // Apply only the "from" date filter
                {
                    query = query.Where(e => e.DisbursementDate.HasValue &&
                                             e.DisbursementDate.Value >= fromDate.Value);
                }
                else if (toDate.HasValue) // Apply only the "to" date filter
                {
                    query = query.Where(e => e.DisbursementDate.HasValue &&
                                             e.DisbursementDate.Value <= toDate.Value);
                }

                // Get total records after filtering
                var totalRecords = query.Count();

                // Apply pagination
                var employeeReports = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                // Calculate total pages
                var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

                // Pass pagination info to the view
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;

                // Return the filtered and paginated employee reports to the view
                return View(employeeReports);
            }
        }

        // Route: GET /report/download-employee-report
        [Route("download-employee")]
        public ActionResult DownloadEmployeeReport()
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                // Query the employee and salary data just like in EmployeeReport
                var query = from e in session.Query<Employee>()
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

                // Set up the PDF document
                MemoryStream workStream = new MemoryStream();
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter.GetInstance(document, workStream).CloseStream = false;

                document.Open();

                // Adding Title
                var titleFont = FontFactory.GetFont("Arial", 16, Font.BOLD);
                var regularFont = FontFactory.GetFont("Arial", 12, Font.NORMAL);
                document.Add(new Paragraph("Employee Report", titleFont));
                document.Add(new Paragraph("Generated on: " + DateTime.Now.ToString("dd/MM/yyyy"), regularFont));
                document.Add(new Chunk("\n"));

                // Creating the table
                PdfPTable table = new PdfPTable(7); // 7 columns for your employee details
                table.WidthPercentage = 100;

                // Adding headers
                var boldFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                table.AddCell(new PdfPCell(new Phrase("First Name", boldFont)));
                table.AddCell(new PdfPCell(new Phrase("Last Name", boldFont)));
                table.AddCell(new PdfPCell(new Phrase("Email", boldFont)));
                table.AddCell(new PdfPCell(new Phrase("Designation", boldFont)));
                table.AddCell(new PdfPCell(new Phrase("Salary", boldFont)));
                table.AddCell(new PdfPCell(new Phrase("Disbursement Date", boldFont)));
                table.AddCell(new PdfPCell(new Phrase("Salary Status", boldFont)));

                // Adding rows
                foreach (var employee in employeeReports)
                {
                    table.AddCell(new PdfPCell(new Phrase(employee.FirstName)));
                    table.AddCell(new PdfPCell(new Phrase(employee.LastName)));
                    table.AddCell(new PdfPCell(new Phrase(employee.Email)));
                    table.AddCell(new PdfPCell(new Phrase(employee.Designation)));
                    table.AddCell(new PdfPCell(new Phrase(employee.Salary.ToString("C"))));
                    table.AddCell(new PdfPCell(new Phrase(employee.DisbursementDate.HasValue ? employee.DisbursementDate.Value.ToString("dd/MM/yyyy") : "No disbursement")));
                    table.AddCell(new PdfPCell(new Phrase(employee.SalaryStatus.HasValue ? employee.SalaryStatus.ToString() : "No status")));
                }

                // Add the table to the document
                document.Add(table);
                document.Close();

                // Returning the PDF file as a download
                byte[] byteInfo = workStream.ToArray();
                workStream.Write(byteInfo, 0, byteInfo.Length);
                workStream.Position = 0;

                return File(workStream, "application/pdf", "EmployeeReport.pdf");
            }
        }

        // Route: GET /report/beneficiary-report
        [Route("beneficiary")]
        public ActionResult BeneficiaryReport(DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 5)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var beneficiaries = session.Query<Beneficiary>()
                                           .FetchMany(b => b.Payments)
                                           .ToList();

                // Filter by payment approval date
                if (fromDate.HasValue)
                {
                    beneficiaries = beneficiaries.Where(b => b.Payments.Any(p => p.PaymentApprovalDate >= fromDate.Value)).ToList();
                }

                if (toDate.HasValue)
                {
                    beneficiaries = beneficiaries.Where(b => b.Payments.Any(p => p.PaymentApprovalDate <= toDate.Value)).ToList();
                }

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

                // Get total count and calculate total pages
                var totalBeneficiaries = viewModel.Count();
                var totalPages = (int)Math.Ceiling((double)totalBeneficiaries / pageSize);

                // Paginate the results
                var paginatedViewModel = viewModel.Skip((page - 1) * pageSize)
                                                   .Take(pageSize)
                                                   .ToList();

                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.FromDate = fromDate; // Add fromDate to ViewBag
                ViewBag.ToDate = toDate; // Add toDate to ViewBag

                return View(paginatedViewModel);
            }
        }


        // Route: GET /report/download-beneficiary-report
        [Route("download-beneficiary")]
        public ActionResult DownloadBeneficiaryReport()
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

                // Generate PDF
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 0f);
                using (MemoryStream stream = new MemoryStream())
                {
                    PdfWriter.GetInstance(document, stream);
                    document.Open();

                    // Title of the PDF
                    Paragraph title = new Paragraph("Beneficiary Report");
                    title.Alignment = Element.ALIGN_CENTER;
                    document.Add(title);
                    document.Add(new Paragraph("\n")); // Line break

                    // Create a table with the appropriate number of columns
                    PdfPTable table = new PdfPTable(8); // Adjust the number of columns based on the fields in the report
                    table.WidthPercentage = 100;

                    // Adding table headers
                    table.AddCell("Beneficiary Name");
                    table.AddCell("Account Number");
                    table.AddCell("Bank IFSC");
                    table.AddCell("Beneficiary Status");
                    table.AddCell("Beneficiary Type");
                    table.AddCell("Amount");
                    table.AddCell("Payment Request Date");
                    table.AddCell("Payment Approval Date");
                    table.AddCell("Payment Status");

                    // Adding data rows
                    foreach (var item in viewModel)
                    {
                        table.AddCell(item.BeneficiaryName);
                        table.AddCell(item.AccountNumber);
                        table.AddCell(item.BankIFSC);
                        table.AddCell(item.BeneficiaryStatus.ToString());
                        table.AddCell(item.BeneficiaryType.ToString());
                        table.AddCell(item.Amount.ToString()); // Formats as currency
                        table.AddCell(item.PaymentRequestDate?.ToString("dd/MM/yyyy") ?? "N/A");
                        table.AddCell(item.PaymentApprovalDate?.ToString("dd/MM/yyyy") ?? "N/A");
                        table.AddCell(item.PaymentStatus.ToString());
                    }

                    document.Add(table);
                    document.Close();

                    byte[] pdfBytes = stream.ToArray();
                    return File(pdfBytes, "application/pdf", "BeneficiaryReport.pdf");
                }
            }
        }

        // Route: GET /report/client-list
        [Route("client-list")]
        public ActionResult ClientList(int page = 1, int pageSize = 10)
        {
            using (var session = NHibernateHelper.CreateSession())
            {
                var query = session.Query<Client>()
                                   .Select(c => new ClientReportDTO
                                   {
                                       Id = c.Id,
                                       CompanyName = c.CompanyName // Assuming your Client model has a CompanyName property
                                   });

                // Get the total count of clients
                var totalClients = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalClients / pageSize);

                // Get the paginated clients
                var clients = query.Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToList();

                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;

                return View(clients);
            }
        }


        // Route: GET /report/employee-report-by-client/{clientId}
        [Route("employee-by-client/{clientId}")]
        public ActionResult EmployeeReportByClient(Guid clientId, int page = 1, int pageSize = 10)
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

                // Get the total count of employees
                var totalEmployees = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalEmployees / pageSize);

                // Get the paginated employee reports
                var employeeReports = query.Skip((page - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToList();

                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = page;
                ViewBag.ClientId = clientId; // Store the client ID for pagination links
                ViewBag.PageSize = pageSize;

                return View(employeeReports);
            }
        }


        // Route: GET /report/beneficiary-report-by-client/{clientId}
        [Route("beneficiary-by-client/{clientId}")]
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
