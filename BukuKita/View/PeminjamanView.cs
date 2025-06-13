using System;
using System.Collections.Generic;
using System.Linq;
using BukuKita.Model;
using static BookLibrary.BookLib;

namespace BukuKita.View
{
    public class PeminjamanView
    {
        public static void PinjamBuku(List<Buku> book, List<Peminjaman> daftarPeminjaman)
        {
            Console.Write("Masukkan ID buku yang ingin dipinjam: ");
            string id = Console.ReadLine()?.Trim().ToUpper();

            var buku = book.FirstOrDefault(b => b.idBuku == id);

            // Table-driven validation: dictionary berisi kondisi dan pesan error
            var validationTable = new Dictionary<Func<bool>, string>
            {
                { () => buku == null, "Buku tidak ditemukan." },
                { () => buku != null && !buku.IsAvailable, "Buku sedang dipinjam." }
            };

            // Loop cek validasi
            foreach (var check in validationTable)
            {
                if (check.Key())
                {
                    Console.WriteLine(check.Value);
                    return;
                }
            }

            Console.Write("Masukkan nama peminjam: ");
            string nama = Console.ReadLine()?.Trim();

            // Membuat objek Peminjaman dan menambah ke daftar peminjaman
            Peminjaman pinjam = new Peminjaman(buku, nama);
            daftarPeminjaman.Add(pinjam);

            // Update status buku
            buku.Borrower = nama;
            buku.BorrowedAt = DateTime.Now;

            // Table-driven output: dictionary dengan key nama output dan aksi output
            var outputTable = new Dictionary<string, Action>
            {
                {
                    "BuktiPeminjaman", () =>
                    {
                        Console.WriteLine("\n--- Bukti Peminjaman ---");
                        Console.WriteLine($"Judul: {buku.judul}, Dipinjam oleh: {nama}, Tanggal: {buku.BorrowedAt.Value.ToShortDateString()}");
                    }
                }
            };

            // Jalankan output bukti peminjaman
            outputTable["BuktiPeminjaman"]();
        }
    }
}
