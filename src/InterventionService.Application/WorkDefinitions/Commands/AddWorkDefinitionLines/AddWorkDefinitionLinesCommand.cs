using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;
using InterventionService.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterventionService.Application.WorkDefinitions.Commands.AddWorkDefinitionLines
{
    public sealed record AddWorkDefinitionLinesCommand(
     Guid WorkDefinitionId,
     IReadOnlyCollection<AddWorkDefinitionLineItem> Lines
 ) : IRequest<Result<WorkDefinitionDto>>;

    public sealed record AddWorkDefinitionLineItem(
        WorkDefinitionLineType Type,
        string Label,
        decimal Quantity,
        Guid? ProductId,
        decimal? UnitPriceExclTax,
        decimal? VatRate,
        int SortOrder
    );
}
