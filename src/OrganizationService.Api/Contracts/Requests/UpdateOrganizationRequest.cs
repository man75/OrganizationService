using OrganizationService.Domain.Enums;

namespace OrganizationService.Api.Contracts.Requests;

public record UpdateOrganizationRequest(
    string Name,
    OrganizationType Type,
    string? Siret
);
