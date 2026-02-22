using System.Net.Http.Json;
using InterventionService.Application.Abstractions;

namespace InterventionService.Infrastructure.Gateways;

public sealed class StockGateway(HttpClient http) : IStockGateway
{
    public async Task<ProductPricingDto?> GetProductPricingAsync(Guid organizationId, Guid productId, CancellationToken ct)
    {
        return new ProductPricingDto(productId, "Produit introuvable ou erreur API", 10m, 0m);
        // adapte l'URL selon ton StockService
        var url = $"stock/products/{productId}/pricing";

        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Add("X-Organization-Id", organizationId.ToString());

        var res = await http.SendAsync(req, ct);
        if (!res.IsSuccessStatusCode) return null;

        return await res.Content.ReadFromJsonAsync<ProductPricingDto>(cancellationToken: ct);
    }
}