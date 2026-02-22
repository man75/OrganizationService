
using InterventionService.Domain.Enums; // Ajustez selon votre namespace réel

namespace InterventionService.Application.Common.Extensions;

public static class WorkLineTypeExtensions
{
    /// <summary>
    /// Convertit le type de ligne d'une définition (modèle) vers le type de ligne d'une commande réelle.
    /// </summary>
    public static WorkOrderLineType ToOrderLineType(this WorkDefinitionLineType source)
    {
        if ((int)source == 0)
            return WorkOrderLineType.Product;

        return source switch
        {
            WorkDefinitionLineType.Labor => WorkOrderLineType.Labor,
            WorkDefinitionLineType.Part => WorkOrderLineType.Part,
            WorkDefinitionLineType.Service => WorkOrderLineType.Service,
            WorkDefinitionLineType.Product => WorkOrderLineType.Product,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}