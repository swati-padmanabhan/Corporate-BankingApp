using CorporateBankingApp.Models;
using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;

namespace CorporateBankingApp.Mappings
{
    public class DocumentMap : ClassMap<Document>
    {
        public DocumentMap()
        {
            Table("Documents");
            Id(d => d.Id).GeneratedBy.GuidComb();
            Map(d => d.DocumentType).Not.Nullable();
            Map(d => d.FilePath).Not.Nullable();
            Map(d => d.UploadDate).Not.Nullable();
            References(x => x.Client).Column("ClientId").Nullable();
            References(x => x.Beneficiary).Column("BeneficiaryId").Nullable();
        }
    }
}