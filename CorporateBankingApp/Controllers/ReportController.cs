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
using iTextSharp.text;
using iTextSharp.text.pdf;

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
    }
}