using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace BeYourMarket.Model.Models.Mapping
{
    public class PrepaidCardMap : EntityTypeConfiguration<PrepaidCard>
    {
        public PrepaidCardMap()
        {
            // Primary Key
            this.HasKey(t => t.Code);

            // Properties
            this.Property(t => t.NumSerie)
                .IsRequired();

            this.Property(t => t.NumLot)
                .IsRequired();


            // Table & Column Mappings
            this.ToTable("PrepaidCards");
            this.Property(t => t.Code).HasColumnName("Code");
            this.Property(t => t.NumSerie).HasColumnName("NumSerie");
            
            this.Property(t => t.NumLot).HasColumnName("NumLot");
            this.Property(t => t.DateGeneration).HasColumnName("DateGeneration");
            this.Property(t => t.DateFinValidite).HasColumnName("DateFinValidite");
            this.Property(t => t.IsValid).HasColumnName("IsValid");
            this.Property(t => t.IsActif).HasColumnName("IsActif");

        }
    }
}
