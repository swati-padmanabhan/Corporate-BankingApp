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
            Map(b => b.BeneficiaryType).CustomType<BeneficiaryType>().Not.Nullable();

            References(b => b.Client).Column("ClientId").Cascade.None().Nullable();
            HasMany(b => b.Payments).Cascade.All().Inverse();
        }
    }

}