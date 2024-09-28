using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class PaymentMap : ClassMap<Payment>
    {
        public PaymentMap()
        {
            Table("Payments");
            Id(p => p.Id).GeneratedBy.GuidComb();
            Map(p => p.Amount).Not.Nullable();
            Map(p => p.PaymentStatus).CustomType<Status>().Not.Nullable();
            Map(p => p.PaymentRequestDate).Not.Nullable();
            Map(p => p.PaymentApprovalDate).Not.Nullable();
            References(p => p.Beneficiary).Column("BeneficiaryId").Cascade.None().Not.Nullable();
        }
    }
}