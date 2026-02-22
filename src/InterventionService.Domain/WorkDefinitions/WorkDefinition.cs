// File: WorkDefinition.cs
// Namespace à adapter à ton projet (ex: InterventionService.Domain.WorkDefinitions)
using InterventionService.Domain.Enums;
using InterventionService.Domain.Exceptions;
using InterventionService.Domain.WorkDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICareCar.Domain.WorkOrders.Definitions
{
    /// <summary>
    /// Modèle d’intervention (WorkDefinition) = template métier réutilisable.
    /// Sert à préremplir un WorkOrder (Ordre de réparation) en copiant ses lignes.
    /// </summary>
    public sealed class WorkDefinition
    {
        private readonly List<WorkDefinitionLine> _lines = new();
        private WorkDefinition() { } // EF Core

        public Guid Id { get; private set; }
        public Guid OrganizationId { get; private set; }

        public string Name { get; private set; } = default!;
        public InterventionType Type { get; private set; }

        public WorkDefinitionStatus Status { get; private set; }

        // Optionnel (utile en SaaS pro)
        public int? EstimatedMinutes { get; private set; }
        public string? Notes { get; private set; }

        public IReadOnlyCollection<WorkDefinitionLine> Lines => _lines.AsReadOnly();

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        /// <summary>
        /// Constructeur minimal = invariants uniquement.
        /// </summary>
        public WorkDefinition(Guid id, Guid organizationId, string name, InterventionType type)
        {
            if (id == Guid.Empty) throw new DomainException("WorkDefinition id is required.");
            if (organizationId == Guid.Empty) throw new DomainException("OrganizationId is required.");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name is required.");

            Id = id;
            OrganizationId = organizationId;

            Name = name.Trim();
            Type = type;

            Status = WorkDefinitionStatus.Active;

            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // --------------------
        // Lifecycle
        // --------------------
        public void Rename(string name)
        {
            EnsureEditable();

            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");

            Name = name.Trim();
            Touch();
        }

        public void Activate()
        {
            if (Status == WorkDefinitionStatus.Active) return;
            Status = WorkDefinitionStatus.Active;
            Touch();
        }

        public void Archive()
        {
            if (Status == WorkDefinitionStatus.Archived) return;
            Status = WorkDefinitionStatus.Archived;
            Touch();
        }

        public void SetEstimatedMinutes(int? minutes)
        {
            EnsureEditable();

            if (minutes.HasValue && minutes.Value <= 0)
                throw new DomainException("EstimatedMinutes must be positive.");

            EstimatedMinutes = minutes;
            Touch();
        }

        public void SetNotes(string? notes)
        {
            EnsureEditable();

            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        // --------------------
        // Lines (template lines)
        // --------------------
        public Guid AddLine(
            WorkDefinitionLineType type,
            string label,
            decimal quantity,
            Guid? productId = null,
            decimal? unitPriceExclTax = null,
            decimal? vatRate = null,
            int sortOrder = 0)
        {
            EnsureEditable();

            if (string.IsNullOrWhiteSpace(label))
                throw new DomainException("Line label is required.");

            if (quantity <= 0)
                throw new DomainException("Quantity must be > 0.");

            if (type == WorkDefinitionLineType.Product && (!productId.HasValue || productId.Value == Guid.Empty))
                throw new DomainException("ProductId is required for product lines.");

            if (productId.HasValue && productId.Value == Guid.Empty)
                throw new DomainException("ProductId is invalid.");

            if (unitPriceExclTax.HasValue && unitPriceExclTax.Value < 0)
                throw new DomainException("UnitPriceExclTax must be >= 0.");

            if (vatRate.HasValue && (vatRate.Value < 0 || vatRate.Value > 100))
                throw new DomainException("VatRate must be between 0 and 100.");

            var line = new WorkDefinitionLine(
                id: Guid.NewGuid(),
                type: type,
                label: label.Trim(),
                quantity: quantity,
                productId: productId,
                unitPriceExclTax: unitPriceExclTax,
                vatRate: vatRate,
                sortOrder: sortOrder
            );

            _lines.Add(line);
            Touch();
            return line.Id;
        }

        public void UpdateLine(
            Guid lineId,
            decimal quantity,
            string? label = null,
            Guid? productId = null,
            decimal? unitPriceExclTax = null,
            decimal? vatRate = null,
            int? sortOrder = null)
        {
            EnsureEditable();

            var line = GetLine(lineId);

            if (quantity <= 0)
                throw new DomainException("Quantity must be > 0.");

            if (!string.IsNullOrWhiteSpace(label))
                line.Rename(label.Trim());

            if (sortOrder.HasValue)
                line.SetSortOrder(sortOrder.Value);

            // règle: si Product -> ProductId obligatoire
            if (line.Type == WorkDefinitionLineType.Product)
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

            line.UpdatePricing(quantity, productId, unitPriceExclTax, vatRate);

            Touch();
        }

        public void RemoveLine(Guid lineId)
        {
            EnsureEditable();

            var line = GetLine(lineId);
            _lines.Remove(line);
            Touch();
        }

        public void ClearLines()
        {
            EnsureEditable();

            _lines.Clear();
            Touch();
        }

        private WorkDefinitionLine GetLine(Guid lineId)
        {
            if (lineId == Guid.Empty) throw new DomainException("LineId is required.");

            var line = _lines.FirstOrDefault(x => x.Id == lineId);
            if (line is null) throw new DomainException("Line not found.");

            return line;
        }

        private void EnsureEditable()
        {
            if (Status == WorkDefinitionStatus.Archived)
                throw new DomainException("Archived definitions are not editable.");
        }

        private void Touch() => UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Ligne de modèle d’intervention.
    /// Pricing optionnel : le prix réel peut être recalculé au moment de la copie vers WorkOrder.
    /// </summary>
 /*   public sealed class WorkDefinitionLine
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
    */
    // --------------------
    // Enums (à adapter à ton domaine)
    // --------------------
   /* public enum WorkDefinitionStatus
    {
        Active = 1,
        Archived = 2
    }

    public enum WorkDefinitionLineType
    {
        Product = 1,
        Labor = 2
    }*/


  
}