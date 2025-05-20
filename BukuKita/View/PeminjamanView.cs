using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
using BukuKita.Model;
using static System.Reflection.Metadata.BlobBuilder;

namespace BukuKita.View
{
    public class PeminjamanView
    {
        public static void PinjamBuku(List<Buku> book, List<Peminjaman> daftarPeminjaman)
        {
            Console.Write("Masukkan ID buku yang ingin dipinjam: ");
            string id = Console.ReadLine()?.Trim().ToUpper();

            var buku = book.FirstOrDefault(b => b.idBuku == id);
            if (book == null || !buku.IsAvailable)
            {
                Console.WriteLine("Buku tidak ditemukan atau sedang dipinjam.");
                return;
            }

            Console.Write("Masukkan nama peminjam: ");
            string nama = Console.ReadLine()?.Trim();

            Peminjaman pinjam = new Peminjaman(buku, nama);
            daftarPeminjaman.Add(pinjam);

            buku.Borrower = nama;
            buku.BorrowedAt = DateTime.Now;

            Console.WriteLine("\n--- Bukti Peminjaman ---");
            Console.WriteLine($"Judul: {buku.judul}, Dipinjam oleh: {nama}, Tanggal: {buku.BorrowedAt.Value.ToShortDateString()}");
        }
    }
}
