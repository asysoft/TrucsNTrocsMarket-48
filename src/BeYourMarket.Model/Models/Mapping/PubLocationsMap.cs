using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models.Mapping
{
    public class PubLocationsMap : EntityTypeConfiguration<PubLocations>
    {
        public PubLocationsMap()
        {
            // ID, IdHtml, PubPageName, PubIsFree, PubFileName, PubFileNumber, AspNetUser

            // Primary Key
            this.HasKey(t => new { t.ID });

            // Table & Column Mappings
            this.ToTable("PubLocations");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.IdHtml).HasColumnName("IdHtml");
            this.Property(t => t.PubPageName).HasColumnName("PubPageName");
            this.Property(t => t.PubIsFree).HasColumnName("PubIsFree");
            this.Property(t => t.PubFileName).HasColumnName("PubFileName");
            this.Property(t => t.PubFileNumber).HasColumnName("PubFileNumber");

        }
    }
}
