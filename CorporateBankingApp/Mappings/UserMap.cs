using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class UserMap : ClassMap<User>
    {
        public UserMap()
        {
            Table("Users");
            Id(u => u.Id).GeneratedBy.GuidComb();
            Map(u => u.UserName).Not.Nullable();
            Map(u => u.Password).Not.Nullable();
            Map(u => u.Email).Not.Nullable();
            HasOne(u => u.Role).Cascade.None().PropertyRef(r => r.User).Constrained();
        }
    }
}