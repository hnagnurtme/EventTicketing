using EventTicketing.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Thêm các extension bạn đã viết
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer(); // để Swagger hoạt động

var app = builder.Build();

// Configure middleware pipeline
app.ConfigureEventTicketingPipeline(app.Environment);

app.Run();
