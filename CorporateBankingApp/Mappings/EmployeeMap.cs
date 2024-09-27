using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employees");
            Id(x => x.Id).GeneratedBy.GuidComb();
            Map(x => x.FirstName).Not.Nullable();
            Map(x => x.LastName).Not.Nullable();
            Map(x => x.Email).Not.Nullable();
            Map(x => x.Designation).Not.Nullable();
            Map(x => x.Phone).Not.Nullable();
            Map(x => x.IsActive).Not.Nullable();
            HasMany(x => x.SalaryDisbursements).Cascade.All().Inverse();
            References(x => x.Client).Column("ClientId").Cascade.None().Not.Nullable();

        }
    }
}