using MediatR;
using OrganizationService.Domain.Enums;

namespace OrganizationService.Application.Organizations.Commands.ActivateOrganization;

public record ActivateOrganizationCommand(
    Guid Id,
    Guid UserId
) : IRequest;
