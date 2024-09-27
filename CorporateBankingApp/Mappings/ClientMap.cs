﻿using CorporateBankingApp.Enums;
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
            Map(x => x.CompanyName).Not.Nullable();
            Map(x => x.ContactInformation).Nullable();
            Map(x => x.Location).Nullable();
            Map(x => x.Balance).Not.Nullable();
            Map(x => x.IsActive).Not.Nullable();
            Map(x => x.OnBoardingStatus).CustomType<Status>().Not.Nullable();
            HasMany(x => x.Beneficiaries).Cascade.All().Inverse();
            HasMany(x => x.Documents).Cascade.All().Inverse().Fetch.Join();
            HasMany(x => x.Reports).Cascade.All().Inverse();
            HasMany(x => x.Employees).Cascade.All().Inverse();
        }
    }
}