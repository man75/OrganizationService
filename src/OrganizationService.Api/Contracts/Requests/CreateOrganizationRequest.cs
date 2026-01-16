using OrganizationService.Domain.Enums;

namespace OrganizationService.Api.Contracts.Requests;

public record CreateOrganizationRequest(
    Guid CreatorUserId,
    string Name,
    OrganizationType Type,
    string? Siret
);
