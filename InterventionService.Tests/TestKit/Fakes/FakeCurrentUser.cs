using InterventionService.Application.Abstractions;
using InterventionService.Application.Common;

namespace InterventionService.Tests.TestKit.Fakes;

public sealed class FakeCurrentUser : IUserContext
{
    public Guid UserId { get; init; } = Guid.NewGuid();
    public Guid OrganizationId { get; init; } = Guid.NewGuid();

    public bool IsAuthenticated => throw new NotImplementedException();
}
