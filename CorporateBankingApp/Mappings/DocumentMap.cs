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
            Id(x => x.Id).GeneratedBy.GuidComb();
            //Map(x => x.DocumentType).Not.Nullable();
            //Map(x => x.FilePath).Not.Nullable();

            Map(x=>x.DocumentName).Not.Nullable();
            Map(x=>x.DocumentLink).Not.Nullable();
            Map(x => x.UploadDate).Not.Nullable();
            References(x => x.Client).Column("ClientId").Cascade.None().Not.Nullable();
        }
    }
}