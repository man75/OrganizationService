using Microsoft.EntityFrameworkCore;
using OrganizationService.Infrastructure.Persistence;
using MediatR;
using OrganizationService.Application.Abstractions;
using OrganizationService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(OrganizationService.Application.Organizations.Commands.CreateOrganization.CreateOrganizationCommand).Assembly));

builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();

builder.Services.AddDbContext<OrganizationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("OrganizationDb")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
