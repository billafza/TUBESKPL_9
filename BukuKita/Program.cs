using BukuKita;

var builder = WebApplication.CreateBuilder(args);

// Check if running in console mode
if (args.Length == 0 || args[0].ToLower() != "api")
{
    // Run console mode
    Console.WriteLine("=== BukuKita Console Mode ===");
    Console.WriteLine("Gunakan 'dotnet run api' untuk menjalankan mode Web API");
    Console.WriteLine();

    MainMenu mainMenu = new MainMenu();
    mainMenu.DisplayMainMenu();

    Console.WriteLine("Terima kasih telah menggunakan BukuKita!");
    return;
}

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
});
builder.Services.AddSingleton<MainMenu>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BukuKita Approval API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => new
{
    status = "healthy",
    timestamp = DateTime.Now,
    application = "BukuKita API",
    version = "1.0"
});

Console.WriteLine("=== BukuKita Web API Mode ===");
Console.WriteLine("Swagger UI: https://localhost:5001/swagger");
Console.WriteLine("Health Check: /health");
Console.WriteLine();

app.Run();