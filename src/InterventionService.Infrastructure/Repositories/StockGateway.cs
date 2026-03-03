using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using InterventionService.Application.Abstractions;
using InterventionService.Application.Common;
using InterventionService.Application.DTOs;

namespace InterventionService.Infrastructure.Gateways;

public sealed class StockGateway : IStockGateway
{
    private readonly HttpClient _http;
    private readonly IUserContext _userContext;

    public StockGateway(HttpClient http, IUserContext userContext)
    {
        _http = http;
        _userContext = userContext;
    }

    public async Task<ProductPricingDto?> GetProductPricingAsync(Guid organizationId, Guid productId, CancellationToken ct)
    {
        try
        {
            // L'URL via la Gateway (ex: http://gateway:8080/api/stock/...)
            var url = "http://localhost:8080/api/stock/prices/effective?articleId=18bed9fe-ae27-4d44-b63b-3b8852679eb9&locationId=313fb0c7-69a6-4ff7-b0a3-05b8e2b10058";// $"api/stock/products/{productId}/pricing";

           var req = new HttpRequestMessage(HttpMethod.Get, url);

            // 1. Passage de l'Organisation ID (Header technique)
            req.Headers.Add("X-Org-Id", organizationId.ToString());

            // 2. Passage du Token JWT (Indispensable pour que la Gateway valide l'appel)
            // On suppose que IUserContext a une propriété 'Token' (récupérée du header Authorization entrant)
            var token = _userContext.Token;
            if (!string.IsNullOrEmpty(token))
            {
                // On retire le préfixe "Bearer " s'il est déjà présent pour éviter les doublons
                var cleanToken = token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? token.Substring(7)
                    : token;

                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", cleanToken);
            }

            var res = await _http.SendAsync(req, ct);

            if (!res.IsSuccessStatusCode)
                return new ProductPricingDto(productId, "Produit introuvable (Stock)", 0m, 0m);

            return await res.Content.ReadFromJsonAsync<ProductPricingDto>(cancellationToken: ct);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StockGateway] Error: {ex.Message}");
            return new ProductPricingDto(productId, "Erreur Service Stock", 0m, 0m);
        }
    }
}