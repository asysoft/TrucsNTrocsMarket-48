using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace BeYourMarket.Model.Models.Mapping
{
    public class AspNetUserCategoryMap : EntityTypeConfiguration<AspNetUserCategory>
    {
        public AspNetUserCategoryMap()
        {
            // Primary Key
            this.HasKey(t => new { t.AspNetUserId, t.CategoryID } );

            // Table & Column Mappings
            this.ToTable("AspNetUserCategories");
            this.Property(t => t.AspNetUserId).HasColumnName("AspNetUserId");      
            this.Property(t => t.CategoryID).HasColumnName("CategoryID");

        }
    }
}
