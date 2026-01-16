using MediatR;
using OrganizationService.Domain.Enums;

namespace OrganizationService.Application.Organizations.Commands.CreateOrganization;

public record CreateOrganizationCommand(
    Guid CreatorUserId,
    string Name,
    OrganizationType Type,
    string? Siret
) : IRequest<Guid>;
