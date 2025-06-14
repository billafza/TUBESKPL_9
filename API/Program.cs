using API.Services;
using API.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BukuKita API",
        Version = "v1",
        Description = "Independent API untuk sistem perpustakaan BukuKita"
    });
    c.EnableAnnotations();
});

// Register repositories
builder.Services.AddSingleton<IApprovalRepository, InMemoryApprovalRepository>();
builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

// Register services
builder.Services.AddScoped<ApprovalService>();
builder.Services.AddScoped<BookService>();

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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BukuKita API v1");
        c.RoutePrefix = string.Empty;
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
    application = "BukuKita Independent API",
    version = "1.0"
});

Console.WriteLine("=== BukuKita Independent API ===");
Console.WriteLine("Swagger UI: http://localhost:5000");
Console.WriteLine("Health Check: /health");

app.Run();