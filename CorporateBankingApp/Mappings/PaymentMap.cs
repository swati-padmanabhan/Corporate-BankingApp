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
            Id(x => x.Id).GeneratedBy.GuidComb();

            Map(x => x.Amount).Not.Nullable();

            Map(x => x.PaymentStatus).CustomType<CompanyStatus>().Not.Nullable();

            Map(x => x.PaymentRequestDate).Not.Nullable();

            Map(x => x.PaymentApprovalDate).Nullable();

            Map(x => x.RazorpayPaymentId);
            Map(x => x.ClientId);

            References(x => x.Beneficiary).Column("BeneficiaryId").Cascade.None().Not.Nullable();
        }
    }
}