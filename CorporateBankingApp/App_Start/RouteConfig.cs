using System.Web.Mvc;
using System.Web.Routing;

namespace CorporateBankingApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route for UserController
            routes.MapRoute(
                name: "UserLogin",
                url: "user/login",
                defaults: new { controller = "User", action = "Login" }
            );

            routes.MapRoute(
                name: "UserRegister",
                url: "user/register",
                defaults: new { controller = "User", action = "Register" }
            );

            routes.MapRoute(
                name: "UserLogout",
                url: "user/logout",
                defaults: new { controller = "User", action = "Logout" }
            );

            routes.MapRoute(
                name: "EditRegistrationSuccess",
                url: "edit-registration-success",
                defaults: new { controller = "User", action = "EditRegistrationSuccess" }
            );
            routes.MapRoute(
                name: "EditClientRegistrationDetails",
                url: "user/edit-registration",
                defaults: new { controller = "User", action = "EditClientRegistrationDetails" }
            );

            // Routes for ClientController
            routes.MapRoute(
                name: "ClientIndex",
                url: "client",
                defaults: new { controller = "Client", action = "Index" }
            );

            routes.MapRoute(
                name: "UserProfile",
                url: "client/user-profile",
                defaults: new { controller = "Client", action = "UserProfile" }
            );



            routes.MapRoute(
                name: "UpdateBalance",
                url: "client/update-balance",
                defaults: new { controller = "Client", action = "UpdateBalance" }
            );

            routes.MapRoute(
                name: "ManageBeneficiaries",
                url: "client/manage-beneficiaries",
                defaults: new { controller = "Client", action = "ManageBeneficiaries" }
            );

            routes.MapRoute(
                name: "GetAllOutboundBeneficiaries",
                url: "client/get-all-beneficiaries",
                defaults: new { controller = "Client", action = "GetAllOutboundBeneficiaries" }
            );

            routes.MapRoute(
                name: "InboundBeneficiaries",
                url: "client/inbound-beneficiaries",
                defaults: new { controller = "Client", action = "InboundBeneficiaries" }
            );

            routes.MapRoute(
                name: "UpdateBeneficiaryStatus",
                url: "client/update-beneficiary-status/{id}/{isActive}",
                defaults: new { controller = "Client", action = "UpdateBeneficiaryStatus" }
            );

            routes.MapRoute(
                name: "AddNewBeneficiary",
                url: "client/add-new-beneficiary",
                defaults: new { controller = "Client", action = "AddNewBeneficiary" }
            );

            routes.MapRoute(
                name: "GetBeneficiaryById",
                url: "client/get-beneficiary-by-id/{id}",
                defaults: new { controller = "Client", action = "GetBeneficiaryById" }
            );

            routes.MapRoute(
                name: "EditBeneficiary",
                url: "client/edit-beneficiary",
                defaults: new { controller = "Client", action = "EditBeneficiary" }
            );

            routes.MapRoute(
                name: "ManageEmployees",
                url: "client/manage-employees",
                defaults: new { controller = "Client", action = "ManageEmployees" }
            );

            routes.MapRoute(
                name: "GetAllEmployees",
                url: "client/get-all-employees",
                defaults: new { controller = "Client", action = "GetAllEmployees" }
            );

            routes.MapRoute(
                name: "AddEmployee",
                url: "client/add-employee",
                defaults: new { controller = "Client", action = "Add" }
            );

            routes.MapRoute(
                name: "GetEmployeeById",
                url: "client/get-employee-by-id/{id}",
                defaults: new { controller = "Client", action = "GetEmployeeById" }
            );

            routes.MapRoute(
                name: "EditEmployee",
                url: "client/edit-employee",
                defaults: new { controller = "Client", action = "Edit" }
            );

            routes.MapRoute(
                name: "UpdateEmployeeStatus",
                url: "client/update-employee-status/{id}/{isActive}",
                defaults: new { controller = "Client", action = "UpdateEmployeeStatus" }
            );

            routes.MapRoute(
                name: "UploadCsv",
                url: "client/upload-csv",
                defaults: new { controller = "Client", action = "UploadCsv" }
            );

            routes.MapRoute(
                name: "ProcessSalaryDisbursements",
                url: "client/salary-disbursements",
                defaults: new { controller = "Client", action = "ProcessSalaryDisbursements" }
            );

            routes.MapRoute(
                name: "MakePaymentRequests",
                url: "client/payment-requests",
                defaults: new { controller = "Client", action = "MakePaymentRequests" }
            );

            routes.MapRoute(
                name: "GetBeneficiaryListForPayment",
                url: "client/get-beneficiary-list-for-payment",
                defaults: new { controller = "Client", action = "GetBeneficiaryListForPayment" }
            );

            routes.MapRoute(
                name: "UploadDocuments",
                url: "client/upload-documents",
                defaults: new { controller = "Client", action = "UploadDocuments" }
            );

            routes.MapRoute(
                name: "GenerateReports",
                url: "client/generate-reports",
                defaults: new { controller = "Client", action = "GenerateReports" }
            );

            // Routes for AdminController
            routes.MapRoute(
                name: "AdminIndex",
                url: "admin",
                defaults: new { controller = "Admin", action = "Index" }
            );

            routes.MapRoute(
                name: "AdminRegistration",
                url: "admin/registration",
                defaults: new { controller = "Admin", action = "AdminRegistration" }
            );

            routes.MapRoute(
                name: "ClientApproval",
                url: "admin/client-approval",
                defaults: new { controller = "Admin", action = "ClientApproval" }
            );

            routes.MapRoute(
                name: "ViewClientDetails",
                url: "admin/client-details/{id}",
                defaults: new { controller = "Admin", action = "ViewClientDetails", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "ApproveClient",
                url: "admin/approve-client",
                defaults: new { controller = "Admin", action = "ApproveClient" }
            );

            routes.MapRoute(
                name: "RejectClient",
                url: "admin/reject-client",
                defaults: new { controller = "Admin", action = "RejectClient" }
            );

            routes.MapRoute(
                name: "ClientManagement",
                url: "admin/client-management",
                defaults: new { controller = "Admin", action = "ClientManagement" }
            );

            routes.MapRoute(
                name: "GetAllClients",
                url: "admin/get-all-clients",
                defaults: new { controller = "Admin", action = "GetAllClients" }
            );

            routes.MapRoute(
                name: "UpdateClientDetails",
                url: "admin/update-client-details/{id}",
                defaults: new { controller = "Admin", action = "UpdateClientDetails", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "DeleteClientDetails",
                url: "admin/delete-client-details/{id}",
                defaults: new { controller = "Admin", action = "DeleteClientDetails", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PaymentApprovals",
                url: "admin/payment-approvals",
                defaults: new { controller = "Admin", action = "PaymentApprovals" }
            );

            routes.MapRoute(
                name: "ApprovePayments",
                url: "admin/approve-payments",
                defaults: new { controller = "Admin", action = "ApprovePayments" }
            );

            routes.MapRoute(
                name: "RejectPayments",
                url: "admin/reject-payments",
                defaults: new { controller = "Admin", action = "RejectPayments" }
            );

            routes.MapRoute(
                name: "BeneficiaryManagement",
                url: "admin/beneficiary-management",
                defaults: new { controller = "Admin", action = "BeneficiaryManagement" }
            );

            routes.MapRoute(
                name: "GetOutboundBeneficiaryForVerification",
                url: "admin/get-beneficiary-for-verification",
                defaults: new { controller = "Admin", action = "GetOutboundBeneficiaryForVerification" }
            );

            routes.MapRoute(
                name: "UpdateOutboundBeneficiaryOnboardingStatus",
                url: "admin/update-beneficiary-status/{id}/{status}",
                defaults: new { controller = "Admin", action = "UpdateOutboundBeneficiaryOnboardingStatus", id = UrlParameter.Optional, status = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SalaryDisbursementApprovals",
                url: "admin/salary-disbursement",
                defaults: new { controller = "Admin", action = "SalaryDisbursementApprovals" }
            );

            routes.MapRoute(
                name: "ApproveDisbursements",
                url: "admin/approve-disbursements",
                defaults: new { controller = "Admin", action = "ApproveDisbursements" }
            );

            routes.MapRoute(
                name: "RejectDisbursements",
                url: "admin/reject-disbursements",
                defaults: new { controller = "Admin", action = "RejectDisbursements" }
            );

            routes.MapRoute(
                name: "ReportGeneration",
                url: "admin/report-generation",
                defaults: new { controller = "Admin", action = "ReportGeneration" }
            );

            // Route for ReportController
            routes.MapRoute(
                name: "EmployeeReport",
                url: "report/employee",
                defaults: new { controller = "Report", action = "EmployeeReport" }
            );

            routes.MapRoute(
                name: "DownloadEmployeeReport",
                url: "report/download-employee",
                defaults: new { controller = "Report", action = "DownloadEmployeeReport" }
            );

            routes.MapRoute(
                name: "BeneficiaryReport",
                url: "report/beneficiary",
                defaults: new { controller = "Report", action = "BeneficiaryReport" }
            );

            routes.MapRoute(
                name: "DownloadBeneficiaryReport",
                url: "report/download-beneficiary",
                defaults: new { controller = "Report", action = "DownloadBeneficiaryReport" }
            );

            routes.MapRoute(
                name: "ClientList",
                url: "report/client-list",
                defaults: new { controller = "Report", action = "ClientList" }
            );

            routes.MapRoute(
                name: "EmployeeReportByClient",
                url: "report/employee-by-client/{clientId}",
                defaults: new { controller = "Report", action = "EmployeeReportByClient", clientId = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "BeneficiaryReportByClient",
                url: "report/beneficiary-by-client/{clientId}",
                defaults: new { controller = "Report", action = "BeneficiaryReportByClient", clientId = UrlParameter.Optional }
            );

            // Route for PaymentController
            routes.MapRoute(
                name: "InitiatePayment",
                url: "payment/initiate",
                defaults: new { controller = "Payment", action = "InitiatePayment" }
            );

            routes.MapRoute(
                name: "PaymentVerification",
                url: "payment/verification",
                defaults: new { controller = "Payment", action = "PaymentVerification" }
            );


            // Default route (can remain as is)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
