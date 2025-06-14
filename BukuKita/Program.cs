// ===== Program.cs (BukuKita Console App) =====
using BukuKita;

Console.WriteLine("=== BukuKita Console Application ===");
Console.WriteLine("Memulai aplikasi perpustakaan...");
Console.WriteLine();

try
{
    MainMenu mainMenu = new MainMenu();
    mainMenu.DisplayMainMenu();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine("Tekan Enter untuk keluar...");
    Console.ReadLine();
}