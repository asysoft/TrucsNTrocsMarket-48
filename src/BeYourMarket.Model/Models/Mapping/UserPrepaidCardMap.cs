using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models.Mapping
{
    public class UserPrepaidCardMap : EntityTypeConfiguration<UserPrepaidCard>
    {
        public UserPrepaidCardMap()
        {
            // Primary Key
            this.HasKey(t => new { t.UserID, t.Code });

            // Table & Column Mappings
            this.ToTable("UserPrepaidCards");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.IsActif).HasColumnName("IsActif");
            this.Property(t => t.DateFirstUse).HasColumnName("DateFirstUse");
            

            //// Relationships
            //this.HasRequired(t => t.AspNetUser)
            //    .WithMany(t => t.UserPrepaidCards)
            //    .HasForeignKey(d => d.UserID).WillCascadeOnDelete();

            //this.HasRequired(t => t.PrepaidCard)
            //    .WithMany(t => t.UserPrepaidCards)
            //    .HasForeignKey(d => d.Code).WillCascadeOnDelete();
        }

    }
}
