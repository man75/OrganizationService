using ICareCar.Domain.WorkOrders.Definitions;


namespace InterventionService.Api.Contracts.Requests
{
    public sealed record AddWorkDefinitionLineRequest(
     Domain.Enums.WorkDefinitionLineType Type,
     string Label,
     decimal Quantity,
     Guid? ProductId,
     decimal? UnitPriceExclTax,
     decimal? VatRate,
     int SortOrder
 );
}
