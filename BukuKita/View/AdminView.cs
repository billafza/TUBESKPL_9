using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
using BukuKita.Model;
namespace BukuKita.View
{
    class AdminView
    {
        public void DisplayMenu(List<Buku> book, List<Peminjaman> daftarPeminjaman, List<Approval> daftarApproval)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n=== MENU ADMIN ===");
                Console.WriteLine("1. Kelola Pengguna");
                Console.WriteLine("2. Kelola Buku");
                Console.WriteLine("0. Logout");
                Console.Write("Pilih: ");
                int input = int.Parse(Console.ReadLine());
                Console.WriteLine();
                switch (input)
                {
                    case 1:
                        Admin.KelolaPengguna(daftarPeminjaman, daftarApproval, book);
                        break;
                    case 2:
                        Buku.KelolaBuku(book);
                        break;
                    case 0:
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("\nPilih menu yang sesuai [1-2] / 0 untuk keluar.");
                        break;
                }
            }
        }
    }
}