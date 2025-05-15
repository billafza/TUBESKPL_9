using System;
using static BookLibrary.BookLib;
using BukuKita.Model;
using BukuKita.View;
using BukuKita.Auth;

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

            List<Peminjaman> daftarPeminjaman = new List<Peminjaman>();
            List<Pengembalian> daftarPengembalian = new List<Pengembalian>();

            bool isRunning = true;
            AuthSystem auth = new AuthSystem();

            while (isRunning)
            {
                Console.WriteLine("\n=== MENU UTAMA ===");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. Register Mahasiswa");
                Console.WriteLine("3. Keluar");
                Console.Write("Pilih opsi (1/2/3): ");
                string pilihan = Console.ReadLine();

                switch (pilihan)
                {
                    case "1":
                        Console.WriteLine("\n=== LOGIN ===");
                        Console.Write("Email: ");
                        string email = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();

                        User userLogin = auth.Login(email, password); // perhatikan: huruf besar "L" di Login

                        if (userLogin != null)
                        {
                            Console.WriteLine($"\nLogin berhasil sebagai {userLogin.role} - {userLogin.nama}");

                            if (userLogin.role.ToLower() == "admin")
                            {
                                AdminView menuAdmin = new AdminView();
                                menuAdmin.displayMenu(book, daftarPeminjaman);
                            }
                            else if (userLogin.role.ToLower() == "mahasiswa")
                            {
                                MahasiswaView menuMhs = new MahasiswaView();
                                menuMhs.displayMenu(book, daftarPeminjaman, daftarPengembalian);
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nEmail atau password salah.");
                        }
                        break;

                    case "2":
                        auth.RegisterMahasiswa();
                        break;

                    case "3":
                        isRunning = false;
                        Console.WriteLine("Terima kasih telah menggunakan BukuKita!");
                        break;

                    default:
                        Console.WriteLine("Opsi tidak valid.");
                        break;
                }

                if (isRunning)
                {
                    Console.Write("\nKembali ke menu utama? (y/n): ");
                    string lanjut = Console.ReadLine();
                    if (lanjut.ToLower() != "y") isRunning = false;
                }
            }

        }
    }
}