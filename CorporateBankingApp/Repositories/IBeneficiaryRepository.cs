using CorporateBankingApp.DTOs;
using CorporateBankingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorporateBankingApp.Repositories
{
    public interface IBeneficiaryRepository
    {
        List<Beneficiary> GetAllOutboundBeneficiaries(Guid clientId);
        void UpdateBeneficiaryStatus(Guid id, bool isActive);
        void AddNewBeneficiary(Beneficiary beneficiary);
        Beneficiary GetBeneficiaryById(Guid id);
        void UpdateBeneficiary(Beneficiary beneficiary);
    }
}
