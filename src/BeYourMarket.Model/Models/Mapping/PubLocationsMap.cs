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
            // Primary Key
            this.HasKey(t => new { t.ID });

            // Table & Column Mappings
            this.ToTable("PubLocations");
            this.Property(t => t.ID).HasColumnName("ID");

            this.Property(t => t.PageName).HasColumnName("PageName");

            this.Property(t => t.TopIsFree).HasColumnName("TopIsFree");
            this.Property(t => t.TopFileName).HasColumnName("TopFileName");
            this.Property(t => t.TopFileNum).HasColumnName("TopFileNum");

            this.Property(t => t.LeftIsFree).HasColumnName("LeftIsFree");
            this.Property(t => t.LeftFileName).HasColumnName("LeftFileName");
            this.Property(t => t.LeftFileNum).HasColumnName("LeftFileNum");

            this.Property(t => t.RightIsFree).HasColumnName("RightIsFree");
            this.Property(t => t.RightFileName).HasColumnName("RightFileName");
            this.Property(t => t.RightFileNum).HasColumnName("RightFileNum");
        }
    }
}
