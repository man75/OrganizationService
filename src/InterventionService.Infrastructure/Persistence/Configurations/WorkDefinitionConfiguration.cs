using InterventionService.Domain.WorkDefinitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterventionService.Infrastructure.Persistence.Configurations;

public sealed class WorkDefinitionConfiguration : IEntityTypeConfiguration<WorkDefinition>
{
    public void Configure(EntityTypeBuilder<WorkDefinition> b)
    {
        b.ToTable("work_definitions");

        b.HasKey(x => x.Id);

        b.Property(x => x.OrganizationId).IsRequired();

        b.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        b.Property(x => x.Type).IsRequired();

        b.Property(x => x.Status).IsRequired();

        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        // Indexes (multi-tenant + perf)
        b.HasIndex(x => new { x.OrganizationId, x.Status });

        // Unicité par org + name (recommandé)
        b.HasIndex(x => new { x.OrganizationId, x.Name }).IsUnique();
    }
}
