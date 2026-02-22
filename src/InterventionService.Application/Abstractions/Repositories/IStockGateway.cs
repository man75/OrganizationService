namespace InterventionService.Application.Abstractions;

public sealed record ProductPricingDto(
    Guid ProductId,
    string Designation,
    decimal UnitPriceExclTax,
    decimal VatRate
);

public interface IStockGateway
{
    Task<ProductPricingDto?> GetProductPricingAsync(Guid organizationId, Guid productId, CancellationToken ct);
}