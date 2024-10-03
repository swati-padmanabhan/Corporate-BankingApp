using CorporateBankingApp.DTOs;
using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Repositories
{
    public interface IAdminRepository
    {
        void CreateAdmin(Admin admin);

        Client GetClientById(Guid id);
        List<Client> GetAllClients();
        void UpdateClientDetails(Client client);
        void DeleteClientDetails(Guid id);
        List<Client> GetPendingClients();


        //salary
        SalaryDisbursement GetSalaryDisbursementById(Guid salaryDisbursementId);

        IEnumerable<SalaryDisbursementDTO> GetSalaryDisbursementsByStatus(CompanyStatus status);

        bool ApproveSalaryDisbursement(Guid salaryDisbursementId);

        bool RejectSalaryDisbursement(Guid salaryDisbursementId);

        //bool ApproveSalaryDisbursement(Guid salaryDisbursementId, bool isBatch = false);

        IEnumerable<PaymentDTO> GetPendingPaymentsByStatus(CompanyStatus status);

        void UpdatePaymentStatus(Guid paymentId, CompanyStatus status);

        void UpdateBeneficiary(Beneficiary beneficiary);

        List<Beneficiary> GetPendingBeneficiaries();

        Beneficiary GetBeneficiaryById(Guid id);
    }
}
