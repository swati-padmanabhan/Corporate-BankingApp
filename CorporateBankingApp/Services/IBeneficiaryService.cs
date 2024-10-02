using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CorporateBankingApp.Services
{
    public interface IBeneficiaryService
    {

        void AddOutboundDetails(BeneficiaryDTO beneficiaryDTO, Client client, IList<HttpPostedFileBase> files);
        //void AddOutboudDetails(BeneficiaryDTO beneficiaryDTO, Client client);
        List<BeneficiaryDTO> GetAllBeneficiaries(Guid clientId);

        Client GetClientById(Guid clientId);

        BeneficiaryDTO GetBeneficiaryById(Guid id);

        void UpdateBeneficiaryDetails(BeneficiaryDTO beneficiaryDTO, Client client);

        void UpdateBeneficiaryStatus(Guid id, bool isActive);
    }
}
