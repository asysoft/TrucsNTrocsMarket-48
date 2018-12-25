using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models.Mapping
{
    public class UsersAddInfoMap : EntityTypeConfiguration<UsersAddInfo>
    {
        public UsersAddInfoMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            this.Property(t => t.UserID)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("UsersAddInfos");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.ProCompany).HasColumnName("ProCompany");
            this.Property(t => t.ProSiret).HasColumnName("ProSiret");
            this.Property(t => t.ProAdress).HasColumnName("ProAdress");
            this.Property(t => t.ProTownZip).HasColumnName("ProTownZip");
            this.Property(t => t.ProPhone).HasColumnName("ProPhone");
            this.Property(t => t.ProSiteWeb).HasColumnName("ProSiteWeb");
            this.Property(t => t.ProEmail).HasColumnName("ProEmail");
            this.Property(t => t.LocationRefID).HasColumnName("LocationRefID");
            this.Property(t => t.ProLongitude).HasColumnName("ProLongitude");
            this.Property(t => t.ProLatitude).HasColumnName("ProLatitude");

            //
            // Relationships
            this.HasRequired(t => t.AspNetUser)
                .WithMany(t => t.UsersAddInfos)
                .HasForeignKey(d => d.UserID).WillCascadeOnDelete();

            this.HasRequired(t => t.LocationRef)
                .WithMany(t => t.UsersAddInfos)
                .HasForeignKey(d => d.LocationRefID).WillCascadeOnDelete();
        }

}

}
