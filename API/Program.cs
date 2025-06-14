using BukuKita;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Web API services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BukuKita Approval API",
        Version = "v1",
        Description = "API untuk mengelola Approval peminjaman buku dalam sistem BukuKita"
    });

    // Enable annotations
    c.EnableAnnotations();

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Register services
builder.Services.AddSingleton<MainMenu>();
builder.Services.AddScoped<ApprovalService>();

// Configure CORS if needed
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BukuKita Approval API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at app's root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.Now,
    application = "BukuKita API",
    version = "1.0",
    environment = app.Environment.EnvironmentName
});

// Add API info endpoint
app.MapGet("/api/info", () => new
{
    name = "BukuKita Approval API",
    version = "1.0",
    description = "API untuk mengelola approval peminjaman buku",
    endpoints = new[]
    {
        "GET /api/approvals - Get all approvals",
        "GET /api/approvals/pending - Get pending approvals",
        "GET /api/approvals/{id} - Get approval by ID",
        "POST /api/approvals - Create new approval",
        "PUT /api/approvals/{id}/process - Process approval",
        "DELETE /api/approvals/{id} - Delete approval"
    }
});

Console.WriteLine("=== BukuKita Web API Mode ===");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Swagger UI: http://localhost:5000 (or https://localhost:5001)");
Console.WriteLine("Health Check: /health");
Console.WriteLine("API Info: /api/info");
Console.WriteLine();

app.Run();