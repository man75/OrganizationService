using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterventionService.Domain.WorkDefinitions
{
    /// <summary>
    /// Ligne de modèle d’intervention.
    /// Pricing optionnel : le prix réel peut être recalculé au moment de la copie vers WorkOrder.
    /// </summary>
    public sealed class WorkDefinitionLine
    {
        private WorkDefinitionLine() { } // EF Core

        public Guid Id { get; private set; }
        public WorkDefinitionLineType Type { get; private set; }

        public string Label { get; private set; } = default!;
        public decimal Quantity { get; private set; }

        public Guid? ProductId { get; private set; }

        // Optionnel dans le template
        public decimal? UnitPriceExclTax { get; private set; }
        public decimal? VatRate { get; private set; }

        public int SortOrder { get; private set; }

        internal WorkDefinitionLine(
            Guid id,
            WorkDefinitionLineType type,
            string label,
            decimal quantity,
            Guid? productId,
            decimal? unitPriceExclTax,
            decimal? vatRate,
            int sortOrder)
        {
            if (id == Guid.Empty) throw new DomainException("Line id is required.");
            if (string.IsNullOrWhiteSpace(label)) throw new DomainException("Line label is required.");
            if (quantity <= 0) throw new DomainException("Quantity must be > 0.");

            if (type == WorkDefinitionLineType.Product && (!productId.HasValue || productId.Value == Guid.Empty))
                throw new DomainException("ProductId is required for product lines.");

            if (productId.HasValue && productId.Value == Guid.Empty)
                throw new DomainException("ProductId is invalid.");

            if (unitPriceExclTax.HasValue && unitPriceExclTax.Value < 0)
                throw new DomainException("UnitPriceExclTax must be >= 0.");

            if (vatRate.HasValue && (vatRate.Value < 0 || vatRate.Value > 100))
                throw new DomainException("VatRate must be between 0 and 100.");

            Id = id;
            Type = type;
            Label = label.Trim();
            Quantity = quantity;

            ProductId = productId;

            UnitPriceExclTax = unitPriceExclTax;
            VatRate = vatRate;

            SortOrder = sortOrder;
        }

        internal void Rename(string label)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new DomainException("Line label is required.");

            Label = label.Trim();
        }

        internal void SetSortOrder(int sortOrder)
        {
            SortOrder = sortOrder;
        }

        internal void UpdatePricing(decimal quantity, Guid? productId, decimal? unitPriceExclTax, decimal? vatRate)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be > 0.");

            if (Type == WorkDefinitionLineType.Product)
            {
                if (!productId.HasValue || productId.Value == Guid.Empty)
                    throw new DomainException("ProductId is required for product lines.");
            }

            if (productId.HasValue && productId.Value == Guid.Empty)
                throw new DomainException("ProductId is invalid.");

            if (unitPriceExclTax.HasValue && unitPriceExclTax.Value < 0)
                throw new DomainException("UnitPriceExclTax must be >= 0.");

            if (vatRate.HasValue && (vatRate.Value < 0 || vatRate.Value > 100))
                throw new DomainException("VatRate must be between 0 and 100.");

            Quantity = quantity;

            // ProductId peut rester null pour Labor
            ProductId = productId;

            UnitPriceExclTax = unitPriceExclTax;
            VatRate = vatRate;
        }
    }

}


