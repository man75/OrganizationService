using InterventionService.Domain.WorkDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICareCar.Infrastructure.Persistence.Configurations
{
    public sealed class WorkDefinitionLineConfiguration
        : IEntityTypeConfiguration<WorkDefinitionLine>
    {
        public void Configure(EntityTypeBuilder<WorkDefinitionLine> builder)
        {
            builder.ToTable("WorkDefinitionLines");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            builder.Property<Guid>("WorkDefinitionId")
                .IsRequired();

            builder.Property(x => x.Type)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Label)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.Quantity)
                .HasPrecision(18, 4)
                .IsRequired();

            builder.Property(x => x.ProductId);

            builder.Property(x => x.UnitPriceExclTax)
                .HasPrecision(18, 2);

            builder.Property(x => x.VatRate)
                .HasPrecision(5, 2);

            builder.Property(x => x.SortOrder)
                .IsRequired();

            // Index pour chargement rapide
            builder.HasIndex("WorkDefinitionId");

            builder.HasIndex(x => x.ProductId);
        }
    }
}