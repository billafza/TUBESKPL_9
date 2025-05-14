using BukuKita.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;

namespace BukuKita.View
{
    class PengembalianView
    {
        public static void KembalikanBuku(List<Peminjaman> daftarPeminjaman, List<Pengembalian> daftarPengembalian)
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
            if (int.TryParse(input, out nomor) && nomor >= 1 && nomor <= daftarPeminjaman.Count)
            {
                var peminjaman = daftarPeminjaman[nomor - 1];
                daftarPeminjaman.RemoveAt(nomor - 1);

                var pengembalian = new Pengembalian(peminjaman.NamaPeminjam, peminjaman.BukuDipinjam.idBuku);
                daftarPengembalian.Add(pengembalian);

                Console.WriteLine("\n--- Bukti Pengembalian ---");
                Console.WriteLine(pengembalian.Info());
            }
            else
            {
                Console.WriteLine("Nomor yang dimasukkan tidak valid.");
            }
        }
    }
}
