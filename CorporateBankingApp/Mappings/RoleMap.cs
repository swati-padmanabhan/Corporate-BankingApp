using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class RoleMap : ClassMap<Role>
    {
        public RoleMap()
        {
            Table("Roles");
            Id(r => r.Id).GeneratedBy.GuidComb();
            Map(r => r.RoleName).Not.Nullable();
            References(r => r.User).Column("UserId").Unique().Cascade.None();
        }
    }
}