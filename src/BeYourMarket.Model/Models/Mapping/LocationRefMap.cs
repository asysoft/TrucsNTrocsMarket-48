using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models.Mapping
{

    public class LocationRefMap : EntityTypeConfiguration<LocationRef>
    {
        public LocationRefMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("LocationsRef");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");

            this.Property(t => t.Parent).HasColumnName("Parent");
            this.Property(t => t.Enabled).HasColumnName("Enabled");
            this.Property(t => t.Ordering).HasColumnName("Ordering");

        }
    }

}
