using MediatR;
using OrganizationService.Application.Abstractions;

namespace OrganizationService.Application.Organizations.Queries.GetOrganization;

public class GetOrganizationHandler(IOrganizationRepository repo)
    : IRequestHandler<GetOrganizationQuery, object?>
{
    public async Task<object?> Handle(GetOrganizationQuery query, CancellationToken ct)
    {
        var org = await repo.GetAsync(query.Id, ct);
        if (org is null) return null;

        // DTO inline (MVP)
        return new
        {
            org.Id,
            org.Name,
            org.Type,
            org.Status,
            org.CreatedAt,
            org.UpdatedAt,
            Members = org.Members.Select(m => new
            {
                m.UserId,
                m.Role,
                m.Status,
                m.CreatedAt
            })
        };
    }
}
