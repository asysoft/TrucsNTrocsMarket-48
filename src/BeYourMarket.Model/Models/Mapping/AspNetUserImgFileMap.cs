using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models.Mapping
{

    public class AspNetUserImgFileMap : EntityTypeConfiguration<AspNetUserImgFile>
    {
        public AspNetUserImgFileMap()
        {
            // Primary Key
            this.HasKey(t => new { t.AspNetUserId, t.PictureID });

            // Properties
            // Table & Column Mappings
            this.ToTable("AspNetUserImgFiles");
            this.Property(t => t.AspNetUserId).HasColumnName("AspNetUserId");
            this.Property(t => t.PictureID).HasColumnName("PictureID");
            this.Property(t => t.Ordering).HasColumnName("Ordering");

            // Relationships
            this.HasRequired(t => t.AspNetUser)
                .WithMany(t => t.AspNetUserImgFiles)
                .HasForeignKey(d => d.AspNetUserId).WillCascadeOnDelete();
            this.HasRequired(t => t.Picture)
                .WithMany(t => t.AspNetUserImgFiles)
                .HasForeignKey(d => d.PictureID).WillCascadeOnDelete();


        }
    }
}
