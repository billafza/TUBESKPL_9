using System;
using static BookLibrary.BookLib;
using BukuKita.Model;
using BukuKita.View;
using BukuKita.Auth;

namespace BukuKita
{
    class Program // Ubah nama kelas dari 'program' ke 'Program' (huruf besar)
    {
        static void Main()
        {
            // Gunakan MainMenu dan runtime configuration
            MainMenu mainMenu = new MainMenu();
            mainMenu.DisplayMainMenu();

            // Setelah keluar dari DisplayMainMenu, program akan berakhir
            Console.WriteLine("Terima kasih telah menggunakan BukuKita!");
        }
    }
}