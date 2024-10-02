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
        Beneficiary GetBeneficiaryById(Guid id);

        List<Beneficiary> GetAllBeneficiaries(Guid clientId);

        void AddNewBeneficiary(Beneficiary beneficiary);

        void EditBeneficiary(Beneficiary beneficiary);

        void UpdateBeneficiaryStatus(Guid id, bool isActive);
    }
}
