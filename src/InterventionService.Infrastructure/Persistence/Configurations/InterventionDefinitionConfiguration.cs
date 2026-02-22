
using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Domain.WorkDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterventionService.Infrastructure.Persistence.Configurations;

public sealed class InterventionDefinitionConfiguration : IEntityTypeConfiguration<WorkDefinition>
{
    public void Configure(EntityTypeBuilder<WorkDefinition> b)
    {
        b.ToTable("intervention_definitions");

        b.HasKey(x => x.Id);

        b.Property(x => x.OrganizationId).IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Type).IsRequired();
        b.Property(x => x.Status).IsRequired();

        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        b.HasIndex(x => new { x.OrganizationId, x.Status });
        b.HasIndex(x => new { x.OrganizationId, x.Name });
    }
}
