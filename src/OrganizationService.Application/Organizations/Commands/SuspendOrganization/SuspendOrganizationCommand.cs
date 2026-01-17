using MediatR;
using OrganizationService.Domain.Enums;

namespace OrganizationService.Application.Organizations.Commands.DeleteOrganization;

public record SuspendOrganizationCommand(
    Guid Id,
    Guid UserId
) : IRequest;
