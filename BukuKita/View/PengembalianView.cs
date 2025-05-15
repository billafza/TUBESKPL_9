using BukuKita.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
using static System.Reflection.Metadata.BlobBuilder;

namespace BukuKita.View
{
    class PengembalianView
    {
        public static void KembalikanBuku(List<Buku> buku, List<Peminjaman> daftarPeminjaman, List<Pengembalian> daftarPengembalian)
        {
            if (daftarPeminjaman.Count == 0)
            {
                Console.WriteLine("Tidak ada buku yang sedang dipinjam.");
                return;
            }

            Console.WriteLine("Daftar Buku yang Sedang Dipinjam:");
            for (int i = 0; i < daftarPeminjaman.Count; i++)
            {
                var p = daftarPeminjaman[i];
                Console.WriteLine($"{i + 1}. {p.BukuDipinjam.judul} (ID: {p.BukuDipinjam.idBuku}) dipinjam oleh {p.NamaPeminjam}");
            }

            Console.Write("\nMasukkan nomor buku yang ingin dikembalikan: ");
            string input = Console.ReadLine();
            int nomor;

            Console.Write("Masukkan ID buku yang ingin dikembalikan: ");
            string id = Console.ReadLine()?.Trim().ToUpper();

            var book = buku.FirstOrDefault(b => b.idBuku == id);
            if (book == null || book.IsAvailable)
            {
                Console.WriteLine("Buku tidak ditemukan atau belum dipinjam.");
                return;
            }

            Console.Write("Masukkan nama pengembali: ");
            string nama = Console.ReadLine()?.Trim();
            if (nama != book.Borrower)
            {
                Console.WriteLine($"Buku ini dipinjam oleh {book.Borrower}, bukan {nama}.");
                return;
            }

            Console.Write("Masukkan tanggal pengembalian: ");
            string inputDate = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out nomor) && nomor >= 1 && nomor <= daftarPeminjaman.Count)
            {
                var peminjaman = daftarPeminjaman[nomor - 1];
                daftarPeminjaman.RemoveAt(nomor - 1);

                var pengembalian = new Pengembalian(peminjaman.NamaPeminjam, peminjaman.BukuDipinjam.idBuku);
                daftarPengembalian.Add(pengembalian);
            }
            else
            {
                Console.WriteLine("Nomor yang dimasukkan tidak valid.");
            }

            // Mencoba mengubah input tanggal ke DateTime
            DateTime returnDate;
            bool isValidDate = TryParseFlexibleDate(inputDate, out returnDate);

            if (!isValidDate)
            {
                Console.WriteLine("Tanggal pengembalian tidak valid.");
                return;
            }

            DateTime borrowedDate = book.BorrowedAt.Value;
            int lateDays = (returnDate - borrowedDate).Days;
            string statusPengembalian = lateDays <= 0 ? "Pengembalian tepat waktu." : $"Telat {lateDays} hari.";

            Console.WriteLine("\n--- Bukti Pengembalian ---");
            Console.WriteLine($"Judul: {book.judul}, Dikembalikan oleh: {nama}, Tanggal: {returnDate.ToShortDateString()}");
            Console.WriteLine(statusPengembalian);

            // Reset status peminjaman
            book.Borrower = null;
            book.BorrowedAt = null;
        }

        public static bool TryParseFlexibleDate(string dateString, out DateTime result)
        {
            string[] formats = new string[]
            {
            "dd/MM/yyyy"
            };

            result = DateTime.MinValue;
            return DateTime.TryParseExact(dateString, formats, new CultureInfo("id-ID"), DateTimeStyles.None, out result);
        }
    }
}
