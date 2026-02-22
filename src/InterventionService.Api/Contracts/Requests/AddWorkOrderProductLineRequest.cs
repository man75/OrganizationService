namespace InterventionService.Api.Contracts.Requests
{
    public sealed record AddWorkOrderProductLineRequest(Guid ProductId, decimal Quantity);
}
