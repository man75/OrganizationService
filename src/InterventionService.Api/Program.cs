using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;

using InterventionService.Application.DTOs;
using InterventionService.Application.Abstractions;
using InterventionService.Application.Abstractions.Repositories;
using InterventionService.Application.Common.Behaviors;

using InterventionService.Infrastructure.Persistence;
using InterventionService.Infrastructure.Persistence.Repositories;
using InterventionService.Application.Common;
using StockService.Infrastructure;
using InterventionService.Infrastructure.Gateways;
using InterventionService.Application.WorkOrders.Commands.ApplyWorkDefinition;

// ⚠️ Tu as importé StockService.Infrastructure : enlève-le si tu peux.
// using StockService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("https://localhost:7002");
// --- Controllers + JSON (AVANT Build) ---
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Contexte utilisateur depuis headers (Gateway) ---
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContext, HttpUserContext>();



// --- Persistence (EF Core + PostgreSQL) ---
builder.Services.AddDbContext<InterventionDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("InterventionDb")));

// --- Unit of Work ---
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<InterventionDbContext>());

// --- Repositories ---
builder.Services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
builder.Services.AddScoped<IWorkDefinitionRepository, WorkDefinitionRepository>();
// --- MediatR + Pipeline behaviors ---
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(WorkOrderDto).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ApplyWorkDefinitionCommandHandler).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(SecurityValidationBehavior<,>));
});

builder.Services.AddHttpClient<IStockGateway, StockGateway>(c =>
{
    c.BaseAddress = new Uri(builder.Configuration["ExternalServices:StockApi"]!);
});


var app = builder.Build();

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();   // ✅ manquait
app.Run();
