using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
using BukuKita.Model;

namespace BukuKita.View
{
    public class PeminjamanView
    {
        public static void PinjamBuku(List<Buku> book, List<Peminjaman> daftarPeminjaman)
        {

            Console.WriteLine("Daftar Buku:");
            foreach (var buku in book)
            {
                Console.WriteLine(buku);
            }

            Console.Write("\nMasukkan ID buku yang ingin dipinjam: ");
            string id = Console.ReadLine();
            Buku bukuDipilih = book.FirstOrDefault(b => b.idBuku == id);

            if (bukuDipilih != null)
            {
                Console.Write("Masukkan nama peminjam: ");
                string nama = Console.ReadLine();

                Peminjaman pinjam = new Peminjaman(bukuDipilih, nama);
                daftarPeminjaman.Add(pinjam);

                Console.WriteLine("\n--- Bukti Peminjaman ---");
                Console.WriteLine(pinjam.Info());
            }
            else
            {
                Console.WriteLine("ID buku tidak ditemukan.");
            }
        }

       
    }
}
