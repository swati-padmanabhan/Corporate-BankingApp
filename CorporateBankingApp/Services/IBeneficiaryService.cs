using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CorporateBankingApp.Services
{
    public interface IBeneficiaryService
    {
        List<BeneficiaryDTO> GetAllOutboundBeneficiaries(Guid clientId, UrlHelper urlHelper);

        void UpdateBeneficiaryStatus(Guid id, bool isActive);

        void AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles);

        Beneficiary GetBeneficiaryById(Guid id);

        void UpdateBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles);

        List<ClientDTO> GetAllInboundBeneficiaries(Guid clientId);
        void AddInboundBeneficiary(Guid clientId, Guid id);
    }
}
