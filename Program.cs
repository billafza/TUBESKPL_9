using System;
using static BookLibrary.Library;
using static UserLibrary.Library;

namespace BukuKita
{
    class program
    {
        static void Main()
        {
            List<Buku> book = new List<Buku>
            {
                new Buku {idBuku = "B01", judul = "Kancil Cerdik", kategori = "Dongeng", penulis = "Cecylia", tahunTerbit = 2010},
                new Buku {idBuku = "B02", judul = "Artificial Intelegence", kategori = "Teknik Komputer", penulis = "Budiono", tahunTerbit = 2015},
                new Buku {idBuku = "B03", judul = "Bunga Sayu", kategori = "Novel", penulis = "Suci Ratna", tahunTerbit = 2020},
                new Buku {idBuku = "B04", judul = "Sejarah Indonesia", kategori = "Sejarah", penulis = "Sri Handayani", tahunTerbit = 2021},
                new Buku {idBuku = "B05", judul = "Sejarah Umum Indonesia", kategori = "Sejarah", penulis = "Sri Suryani", tahunTerbit = 2011}
            };

            bool isRunning = true;
            while (isRunning)
            {
                AdminMenu menuAdmin = new AdminMenu();
                MahasiswaMenu menuMhs = new MahasiswaMenu();

                Console.WriteLine("\n=== LOGIN ===");
                Console.WriteLine("1. Menu Admin");
                Console.WriteLine("2. Menu Mahasiswa");
                Console.WriteLine("0. Logout");
                Console.WriteLine("Pilih: ");
                int input = int.Parse(Console.ReadLine());
                Console.WriteLine();

                switch (input)
                {
                    case 1:
                        menuAdmin.displayMenu(book);
                        break;
                    case 2:
                        menuMhs.displayMenu(book);
                        break;
                    case 0:
                        Console.WriteLine("\nTerima kasih telah menggunakan Buku Kita!");
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nPilih menu yang sesuai.");
                        break;
                }
            }
        }
    }
}