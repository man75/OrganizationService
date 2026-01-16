using MediatR;
using OrganizationService.Domain.Enums;

namespace OrganizationService.Application.Organizations.Commands.UpdateOrganization;

public record UpdateOrganizationCommand(
    Guid Id,
    Guid UserId,
    string Name,
    OrganizationType Type,
    string? Siret
) : IRequest;
