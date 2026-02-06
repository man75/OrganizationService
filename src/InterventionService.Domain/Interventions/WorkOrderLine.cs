using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;

namespace InterventionService.Domain.WorkOrders;

public class WorkOrderLine
{
    private WorkOrderLine() { } // EF

    public Guid Id { get; private set; }
    public WorkOrderLineType Type { get; private set; }

    public Guid? ProductId { get; private set; } // Stock
    public string Label { get; private set; } = default!;

    public decimal Quantity { get; private set; }
    public Money UnitPriceExclTax { get; private set; } = default!; // ✅ fix EF
    public decimal VatRate { get; private set; }

    public int SortOrder { get; private set; }

    public WorkOrderLine(
        Guid id,
        WorkOrderLineType type,
        string label,
        decimal quantity,
        Money unitPriceExclTax,
        decimal vatRate,
        Guid? productId = null,
        int sortOrder = 0)
    {
        if (id == Guid.Empty) throw new DomainException("Line id is required.");
        if (string.IsNullOrWhiteSpace(label)) throw new DomainException("Label is required.");
        if (quantity <= 0) throw new DomainException("Quantity must be > 0.");
        if (vatRate < 0 || vatRate > 1) throw new DomainException("VatRate must be between 0 and 1.");

        if (type == WorkOrderLineType.Part && (!productId.HasValue || productId == Guid.Empty))
            throw new DomainException("ProductId is required for part lines.");

        Id = id;
        Type = type;
        Label = label.Trim();
        Quantity = quantity;
        UnitPriceExclTax = unitPriceExclTax ?? throw new DomainException("UnitPriceExclTax is required.");
        VatRate = vatRate;
        ProductId = productId;
        SortOrder = sortOrder;
    }

    public void UpdatePricing(decimal quantity, Money unitPriceExclTax, decimal vatRate)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be > 0.");
        if (vatRate < 0 || vatRate > 1) throw new DomainException("VatRate must be between 0 and 1.");

        Quantity = quantity;
        UnitPriceExclTax = unitPriceExclTax ?? throw new DomainException("UnitPriceExclTax is required.");
        VatRate = vatRate;
    }

    public void Rename(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
            throw new DomainException("Label is required.");

        Label = label.Trim();
    }

    public Money TotalExclTax()
        => new(Quantity * UnitPriceExclTax.Amount, UnitPriceExclTax.Currency);

    public Money TotalTax()
        => new(TotalExclTax().Amount * VatRate, UnitPriceExclTax.Currency);

    public Money TotalInclTax()
        => new(TotalExclTax().Amount + TotalTax().Amount, UnitPriceExclTax.Currency);
}
