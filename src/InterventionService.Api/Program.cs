using MediatR;
using Microsoft.EntityFrameworkCore;


using InterventionService.Application.DTOs;

using InterventionService.Application.Abstractions; // ICurrentUser, IUnitOfWork
using InterventionService.Application.Abstractions.Repositories; // repos interfaces

using InterventionService.Infrastructure.Persistence; // InterventionDbContext

using InterventionService.Application.Common;
using StockService.Infrastructure;
using InterventionService.Application.Common.Behaviors;
using InterventionService.Infrastructure.Persistence.Repositories;
using System.Text.Json.Serialization; // HttpCurrentUser (à créer)

var builder = WebApplication.CreateBuilder(args);

// --- Services standards ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Contexte utilisateur depuis headers (Gateway) ---
builder.Services.AddHttpContextAccessor();

// ✅ ICurrentUser lu depuis headers
builder.Services.AddScoped<IUserContext, HttpUserContext>();

// --- MediatR + Pipeline behaviors ---
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(WorkOrderDto).Assembly);

    // Si tu veux garder le même pattern que Stock:
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(SecurityValidationBehavior<,>));
    // Tu peux aussi ajouter ValidationBehavior si tu utilises FluentValidation
});

// --- Persistence (EF Core + PostgreSQL) ---
builder.Services.AddDbContext<InterventionDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// --- Unit of Work : DbContext implémente IUnitOfWork ---
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InterventionDbContext>());

// --- Repositories ---
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();

builder.Services.AddScoped<IWorkDefinitionRepository, WorkDefinitionRepository>();

var app = builder.Build();

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
app.Run();
