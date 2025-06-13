using System;
using System.Collections.Generic;
using System.Linq;
using BukuKita.Model;
using static BookLibrary.BookLib;

namespace BukuKita.View
{
    public class PeminjamanView
    {
        public static void PinjamBuku(List<Buku> daftarBuku, List<Peminjaman> daftarPeminjaman)
        {
            // Meminta input ID buku dari pengguna
            Console.Write("Masukkan ID buku yang ingin dipinjam: ");
            string? idBuku = Console.ReadLine()?.Trim().ToUpper();

            // Validasi: pastikan ID buku tidak kosong
            if (string.IsNullOrEmpty(idBuku))
            {
                Console.WriteLine("❌ ID buku tidak boleh kosong.");
                return;
            }

            // Cari buku berdasarkan ID
            Buku? bukuDipilih = daftarBuku.FirstOrDefault(b => b.idBuku == idBuku);

            // Validasi kondisi buku dengan pendekatan table-driven
            var validasi = new Dictionary<Func<bool>, string>
            {
                { () => bukuDipilih == null, "❌ Buku tidak ditemukan." },
                { () => bukuDipilih != null && !bukuDipilih.IsAvailable, "❌ Buku sedang dipinjam." }
            };

            // Jalankan setiap validasi, jika ada yang gagal, tampilkan pesan dan hentikan proses
            foreach (var cek in validasi)
            {
                if (cek.Key())
                {
                    Console.WriteLine(cek.Value);
                    return;
                }
            }

            // Meminta input nama peminjam
            Console.Write("Masukkan nama peminjam: ");
            string? namaPeminjam = Console.ReadLine()?.Trim();

            // Validasi: pastikan nama tidak kosong
            if (string.IsNullOrEmpty(namaPeminjam))
            {
                Console.WriteLine("❌ Nama peminjam tidak boleh kosong.");
                return;
            }

            // Buat objek peminjaman baru dan tambahkan ke daftar
            var peminjamanBaru = new Peminjaman(bukuDipilih, namaPeminjam);
            daftarPeminjaman.Add(peminjamanBaru);

            // Update status buku (dipinjam)
            bukuDipilih.Borrower = namaPeminjam;
            bukuDipilih.BorrowedAt = DateTime.Now;

            // Tampilkan bukti peminjaman kepada pengguna
            TampilkanBuktiPeminjaman(bukuDipilih, namaPeminjam);
        }

        /// <summary>
        /// Menampilkan bukti peminjaman buku ke layar
        /// </summary>
        private static void TampilkanBuktiPeminjaman(Buku buku, string namaPeminjam)
        {
            Console.WriteLine("\n--- Bukti Peminjaman ---");
            Console.WriteLine($"Judul        : {buku.judul}");
            Console.WriteLine($"Dipinjam oleh: {namaPeminjam}");
            Console.WriteLine($"Tanggal      : {buku.BorrowedAt?.ToShortDateString()}");
        }
    }
}
