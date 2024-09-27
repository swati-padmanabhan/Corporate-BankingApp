using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;


namespace CorporateBankingApp.Mappings
{
    public class AdminMap : SubclassMap<Admin>
    {
        public AdminMap()
        {
            Table("Admins");
            KeyColumn("UserId");
            Map(x => x.BankName).Not.Nullable();
        }
    }
}