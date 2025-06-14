using BukuKita;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BukuKita API",
        Version = "v1",
        Description = "API untuk sistem perpustakaan BukuKita"
    });
    c.EnableAnnotations();
});

// Register services
builder.Services.AddSingleton<MainMenu>();
builder.Services.AddScoped<ApprovalService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BukuKita API v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.Now,
    application = "BukuKita API",
    version = "1.0"
});

Console.WriteLine("=== BukuKita API ===");
Console.WriteLine("Swagger UI: http://localhost:5000");
Console.WriteLine("Health Check: /health");

app.Run();