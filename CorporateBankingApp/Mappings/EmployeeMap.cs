using CorporateBankingApp.Models;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Table("Employees");
            Id(e => e.Id).GeneratedBy.GuidComb();
            Map(e => e.FirstName).Not.Nullable();
            Map(e => e.LastName).Not.Nullable();
            Map(e => e.Email).Not.Nullable();
            Map(e => e.Phone).Not.Nullable();
            Map(e => e.Designation).Not.Nullable();
            Map(e => e.IsActive).Not.Nullable();
            HasMany(e => e.SalaryDisbursements).Cascade.All().Inverse();
            References(e => e.Client).Column("ClientId").Cascade.None().Not.Nullable();

        }
    }
}