using MediatR;

namespace OrganizationService.Application.Organizations.Queries.GetOrganization;

public record GetOrganizationQuery(Guid Id) : IRequest<object?>;
