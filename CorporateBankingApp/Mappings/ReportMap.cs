using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class ReportMap : ClassMap<Report>
    {
        public ReportMap()
        {
            Table("Reports");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.GeneratedDate).Not.Nullable();
            Map(x => x.ReportType).Not.Nullable();
            Map(x => x.GeneratedBy).Not.Nullable();
            References(x => x.Client).Column("ClientId").Cascade.None().Not.Nullable();
        }
    }
}