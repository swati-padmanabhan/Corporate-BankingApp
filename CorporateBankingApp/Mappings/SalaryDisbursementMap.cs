using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class SalaryDisbursementMap : ClassMap<SalaryDisbursement>
    {
        public SalaryDisbursementMap()
        {
            Table("SalaryDisbursements");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.Salary).Not.Nullable();
            Map(x => x.DisbursementDate).Not.Nullable();
            Map(x => x.IsBatch).Not.Nullable();
            Map(x => x.SalaryStatus).CustomType<Status>().Not.Nullable();
            References(x => x.Employee).Column("EmployeeId").Cascade.None().Not.Nullable();
        }
    }
}