using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;

namespace BukuKita.Model
{
    public class Admin : User
    {
        private string ID { get; set; }

        //Default konstruktor
        public Admin() { }

        //Konstruktor
        public Admin(string nama, String ID, EnumJenisKelamin jnsKelamin, string email, string password, string role) : base(nama, jnsKelamin, email, password, role)
        {
            this.ID = ID;
        }

        public static void KelolaPengguna(List<Peminjaman> DaftarPeminjaman)
        {
            bool isRunning = true;
            while (isRunning)
            {

                Console.WriteLine("\n=== MENU KELOLA PENGGUNA ===");
                Console.WriteLine("1. Melihat Semua Pengguna");
                Console.WriteLine("2. Menambah Pengguna");
                Console.WriteLine("3. Menghapus Pengguna");
                Console.WriteLine("4. Melihat Data Peminjaman");
                Console.WriteLine("0. Kembali");
                Console.WriteLine("Pilih: ");
                int pilih = int.Parse(Console.ReadLine());
                Console.WriteLine();

                switch (pilih)
                {
                    case 1:
                        Console.WriteLine("1. Melihat Semua Pengguna");
                        break;

                    case 2:
                        Console.WriteLine("2. Menambah Pengguna");
                        break;

                    case 3:
                        Console.WriteLine("3. Menghapus Pengguna");
                        break;

                    case 4:
                        Peminjaman.ShowDataPeminjam(DaftarPeminjaman);
                        break;

                    case 0:
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nPilih menu yang sesuai [1-4] / 0 untuk kembali.");
                        break;
                }
            }
        }
    }
}
