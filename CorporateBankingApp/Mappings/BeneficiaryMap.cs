using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class BeneficiaryMap : ClassMap<Beneficiary>
    {
        public BeneficiaryMap()
        {
            Table("Beneficiaries");
            Id(b => b.Id).GeneratedBy.GuidComb();
            Map(b => b.BeneficiaryName).Not.Nullable();
            Map(b => b.AccountNumber).Not.Nullable();
            Map(b => b.BankIFSC).Not.Nullable();
            Map(e => e.IsActive).Not.Nullable();
            Map(b => b.BeneficiaryType).CustomType<BeneficiaryType>().Not.Nullable();
            Map(c => c.BeneficiaryStatus).CustomType<CompanyStatus>().Not.Nullable();
            HasMany(x => x.Documents).Cascade.All().Inverse();
            References(b => b.Client).Column("ClientId").Cascade.None().Nullable();
            HasMany(b => b.Payments).Cascade.All().Inverse();
        }
    }

}