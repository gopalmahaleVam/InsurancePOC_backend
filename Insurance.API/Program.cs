using Insurance.Application;
using Insurance.Persistence;
using Insurance.Infrastructure;
using Insurance.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add Controllers (REQUIRED)
builder.Services.AddControllers();

// ✅ Swagger (for API testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Clean Architecture DI
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();

var app = builder.Build();

// ✅ Swagger only in Dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Optional (keep for prod, safe for POC)
app.UseHttpsRedirection();

// ✅ Global Exception Handling
app.UseMiddleware<ExceptionMiddleware>();

// ✅ Routing
app.MapControllers();

app.Run();