using InterventionService.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterventionService.Infrastructure.Persistence.Configurations;

public sealed class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> b)
    {
        b.ToTable("work_orders");

        b.HasKey(x => x.Id);

        b.Property(x => x.OrganizationId).IsRequired();
        b.Property(x => x.Kind).IsRequired();
        b.Property(x => x.Status).IsRequired();

        b.Property(x => x.VehicleId);
        b.Property(x => x.ClientId);
        b.Property(x => x.DefinitionId);
        b.Property(x => x.TechnicianId);

        b.Property(x => x.ScheduledAt).IsRequired();
        b.Property(x => x.StartedAt);
        b.Property(x => x.CompletedAt);

        b.Property(x => x.Currency)
            .HasMaxLength(3)
            .IsRequired();

        b.Property(x => x.Notes)
            .HasMaxLength(2000);

        b.Property(x => x.CreatedAt).IsRequired();
        b.Property(x => x.UpdatedAt).IsRequired();

        // ✅ IMPORTANT : Lines utilise le backing field _lines
        b.Navigation(x => x.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ✅ NE PAS référencer "_lines" ici
        b.HasMany(x => x.Lines)
            .WithOne()
            .HasForeignKey("WorkOrderId")
            .OnDelete(DeleteBehavior.Cascade);

        b.HasIndex(x => new { x.OrganizationId, x.Status });
        b.HasIndex(x => new { x.OrganizationId, x.VehicleId });
        b.HasIndex(x => new { x.OrganizationId, x.ClientId });
        b.HasIndex(x => new { x.OrganizationId, x.ScheduledAt });
        b.HasIndex(x => new { x.OrganizationId, x.DefinitionId });
    }
}
