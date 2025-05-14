using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
using BukuKita.Model;

namespace BukuKita.View
{
    class MahasiswaView
    {
        //Method untuk menampilkan menu Mahasiswa
        public void displayMenu(List<Buku> book, List<Peminjaman> daftarPeminjaman, List<Pengembalian> daftarPengembalian)
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
                        Console.Write("Masukkan ID/Judul/Penulis/Kategori buku: ");
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
                        PeminjamanView.PinjamBuku(book, daftarPeminjaman);
                        break;

                    case 4:
                        PengembalianView.KembalikanBuku(daftarPeminjaman, daftarPengembalian);
                        break;

                    case 5:
                        Buku.TambahBuku(book);
                        break;

                    case 0:
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("\nPilih menu yang sesuai [1-5] / 0 untuk Keluar.");
                        break;
                }
            }
        }
    }
}
