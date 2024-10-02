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
        Beneficiary GetBeneficiaryById(Guid id);

        List<BeneficiaryDTO> GetAllBeneficiaries(Guid clientId, UrlHelper urlHelper);

        void AddNewBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles);

        void EditBeneficiary(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> uploadedFiles);

        void UpdateBeneficiaryStatus(Guid id, bool isActive);
    }
}
