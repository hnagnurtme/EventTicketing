using EventTicketing.API;
using EventTicketing.Infrastructure;
using EventTicketing.Application;
using EventTicketing.API.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddSwaggerDocumentation()
    .AddAuthenticationInfrastructure(builder.Configuration)
    .AddCorsPolicy();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// --- Serilog ---
builder.Logging.ClearProviders(); 

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});
var app = builder.Build();

// --- Configure pipeline ---
app.ConfigureEventTicketingPipeline(builder.Environment);

app.Run();
