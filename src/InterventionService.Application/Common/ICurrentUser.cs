namespace InterventionService.Application.Common;

public interface IUserContext
{
    Guid UserId { get; }
    Guid OrganizationId { get; }
    bool IsAuthenticated { get; }
}
