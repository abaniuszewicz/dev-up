using DevUp.Api;
using DevUp.Api.Minimal;
using DevUp.Application;
using DevUp.Domain;
using DevUp.Infrastructure;
using DevUp.Infrastructure.Http;
using DevUp.Infrastructure.Postgres;
using DevUp.Infrastructure.Postgres.Migrations;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDomain(configuration)
    .AddApplication(configuration)
    .AddInfrastructure()
    .AddHttpInfrastructure()
    .AddPostgresInfrastructure(configuration)
    .AddApi(configuration);

var app = builder.Build();
var environment = app.Environment;

app.UseApi();
app.UseHttpInfrastructure(environment);
app.UseEndpoints();

app.MigrateUp().Run();
