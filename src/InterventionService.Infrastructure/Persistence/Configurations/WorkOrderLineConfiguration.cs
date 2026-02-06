using InterventionService.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterventionService.Infrastructure.Persistence.Configurations;

public sealed class WorkOrderLineConfiguration : IEntityTypeConfiguration<WorkOrderLine>
{
    public void Configure(EntityTypeBuilder<WorkOrderLine> b)
    {
        b.ToTable("work_order_lines");

        b.HasKey(x => x.Id);

        // shadow FK declared in WorkOrderConfiguration
        b.Property<Guid>("WorkOrderId").IsRequired();

        b.Property(x => x.Type).IsRequired();

        b.Property(x => x.ProductId);

        b.Property(x => x.Label)
            .HasMaxLength(250)
            .IsRequired();

        b.Property(x => x.Quantity)
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        b.Property(x => x.VatRate)
            .HasColumnType("numeric(5,4)")
            .IsRequired();

        b.Property(x => x.SortOrder).IsRequired();

        b.OwnsOne(x => x.UnitPriceExclTax, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("unit_price_excl_tax_amount")
                .HasColumnType("numeric(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("unit_price_excl_tax_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.HasIndex("WorkOrderId");
    }
}
