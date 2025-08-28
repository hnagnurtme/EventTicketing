using EventTicketing.API.Extensions;
using EventTicketing.Infrastructure;
using EventTicketing.Application;

var builder = WebApplication.CreateBuilder(args);

// Register Infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);
// --- Add services ---
builder.Services
    .AddApplication()                          // Application layer (MediatR, Validators…)
    .AddPresentation()                         // Presentation (Mapping, Filters…)
    .AddSwaggerDocumentation()
    .AddJwtAuthentication(builder.Configuration)
    .AddCorsPolicy();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

// --- Configure pipeline ---
app.ConfigureEventTicketingPipeline(builder.Environment);

app.Run();
