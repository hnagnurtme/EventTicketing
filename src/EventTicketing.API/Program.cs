using EventTicketing.API.Extensions;
using EventTicketing.Infrastructure;
using EventTicketing.Application;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);

builder.Services
    .AddApplication()                          
    .AddPresentation()                        
    .AddSwaggerDocumentation()
    .AddJwtAuthentication(builder.Configuration)
    .AddCorsPolicy();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.ConfigureEventTicketingPipeline(builder.Environment);

app.Run();
