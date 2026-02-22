using ICareCar.Domain.WorkOrders.Definitions;
using InterventionService.Application.Abstractions;
using InterventionService.Domain.WorkDefinitions;
using InterventionService.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;

namespace InterventionService.Infrastructure.Persistence;

public sealed class InterventionDbContext : DbContext, IUnitOfWork
{
    public InterventionDbContext(DbContextOptions<InterventionDbContext> options) : base(options) { }

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<WorkOrderLine> WorkOrderLines => Set<WorkOrderLine>();

    // ? Catalogue types d’intervention
    public DbSet<WorkDefinition> WorkDefinitions => Set<WorkDefinition>();
    public DbSet<WorkDefinitionLine> WorkDefinitionLines => Set<WorkDefinitionLine>();
    public override Task<int> SaveChangesAsync(CancellationToken ct) => base.SaveChangesAsync(ct);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InterventionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
