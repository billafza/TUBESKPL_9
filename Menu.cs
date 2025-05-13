using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.Library;
using static UserLibrary.Library;

namespace BukuKita
{
    public interface Menu
    {
        public void displayMenu(List<Buku> book);
    }

    class AdminMenu : Menu
    {
        public void displayMenu(List<Buku> book)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Admin admin = new Admin();

                Console.WriteLine("\n=== MENU ADMIN ===");
                Console.WriteLine("1. Lihat Semua Pengguna");
                Console.WriteLine("2. Kelola Buku");
                Console.WriteLine("0. Logout");
                Console.WriteLine("Pilih: ");
                int input = int.Parse(Console.ReadLine());
                Console.WriteLine();

                switch (input)
                {
                    case 1:
                        Console.WriteLine("case 1");
                        break;

                    case 2:
                        Buku.KelolaBuku(book);
                        break;

                    case 0:
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nPilih menu yang sesuai.");
                        break;
                }
            }
        }
    }

    class MahasiswaMenu : Menu
    {
        public void displayMenu(List<Buku> book)
        {
            bool isRunning = true;
            while (isRunning)
            {

                Console.WriteLine("\n=== MENU MAHASISWA ===");
                Console.WriteLine("1. Katalog Buku");
                Console.WriteLine("2. Mencari Buku");
                Console.WriteLine("3. Meminjam Buku");
                Console.WriteLine("4. Mengembalikan Buku");
                Console.WriteLine("5. Mengunggah Buku");
                Console.WriteLine("0. Logout");
                Console.WriteLine("Pilih: ");
                int input = int.Parse(Console.ReadLine());
                Console.WriteLine();

                switch (input)
                {
                    case 1:
                        Buku.DaftarBuku(book);
                        break;

                    case 2:
                        Console.Write("Masukkan kata kunci buku: ");
                        string keyword = Console.ReadLine();

                        List<Buku> hasil = Buku.FilterBuku(book, keyword);

                        if (hasil.Count == 0)
                        {
                            Console.WriteLine("Buku tidak ditemukan.");
                        }
                        else
                        {
                            Console.WriteLine("--- Hasil Pencarian Buku : " + keyword + " ---");
                            foreach (var b1 in hasil)
                            {
                                Buku.displayBuku(b1);
                            }
                        }
                        break;

                    case 3:
                        Console.WriteLine("case 3");

                        break;

                    case 4:
                        Console.WriteLine("case 4");

                        break;

                    case 5:
                        Buku.TambahBuku(book);
                        break;

                    case 0:
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
