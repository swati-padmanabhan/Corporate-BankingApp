using CorporateBankingApp.Enums;
using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class ClientMap : SubclassMap<Client>
    {
        public ClientMap()
        {
            Table("Clients");
            KeyColumn("UserId");
            Map(c => c.CompanyName).Not.Nullable();
            Map(c => c.Location).Nullable();
            Map(c => c.ContactInformation).Nullable();
            Map(c => c.OnBoardingStatus).CustomType<Status>().Not.Nullable();
            Map(c => c.IsActive).Not.Nullable();
            Map(c => c.AccountNumber).Not.Nullable();
            Map(c => c.ClientIFSC).Not.Nullable();
            Map(c => c.Balance).Not.Nullable();
            HasMany(c => c.Beneficiaries).Cascade.All().Inverse();
            HasMany(x => x.Documents).Cascade.All().Inverse();
            HasMany(c => c.Reports).Cascade.All().Inverse();
            HasMany(c => c.Employees).Cascade.All().Inverse();
        }
    }
}