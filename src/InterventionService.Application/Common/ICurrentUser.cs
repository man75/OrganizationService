namespace InterventionService.Application.Common;

public interface IUserContext
{
    Guid UserId { get; }
    Guid OrganizationId { get; }
    bool IsAuthenticated { get; }
    // ? Ajout du token pour permettre aux Gateways de le propager
    string? Token { get; }
}
