using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeYourMarket.Model.Models.Mapping
{
    public class PrepaidCardHistoMap : EntityTypeConfiguration<PrepaidCardHisto>
    {
        public PrepaidCardHistoMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Code)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("PrepaidCardsHisto");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.UsrLogin).HasColumnName("UsrLogin");
            this.Property(t => t.DateStatus).HasColumnName("DateStatus");
            this.Property(t => t.IPAccess).HasColumnName("IPAccess");
            this.Property(t => t.LastCardStatus).HasColumnName("LastCardStatus");
            this.Property(t => t.Msg).HasColumnName("Msg");

            this.HasRequired(t => t.PrepaidCardInfo)
                .WithMany(t => t.PrepaidCardsHisto)
                .HasForeignKey(d => d.Code).WillCascadeOnDelete();

        }
    }
}
