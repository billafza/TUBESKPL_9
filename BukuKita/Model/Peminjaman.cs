using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
namespace BukuKita.Model
{
    public class Peminjaman
    {
        public string idPeminjaman { get; set; }
        public Buku BukuDipinjam { get; set; }
        public string NamaPeminjam { get; set; }
        public DateTime TanggalPinjam { get; set; }
        public string status { get; set; } // Tambahkan properti status

        public Peminjaman()
        {
            this.idPeminjaman = $"PJM{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            this.status = "Pending"; // Default status
        }

        public Peminjaman(Buku buku, string nama)
        {
            this.idPeminjaman = $"PJM{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            BukuDipinjam = buku;
            NamaPeminjam = nama;
            TanggalPinjam = DateTime.Now;
            this.status = "Pending"; // Default status
        }

        public string Info()
        {
            return $"Judul: {BukuDipinjam.judul}, Dipinjam oleh: {NamaPeminjam}, Tanggal: {TanggalPinjam.ToShortDateString()}, Status: {status}";
        }

        public static void ShowDataPeminjam(List<Peminjaman> daftarPeminjaman)
        {
            if (daftarPeminjaman.Count != 0)
            {
                Console.WriteLine("=== DATA PEMINJAMAN BUKU ===");
                for (int i = 0; i < daftarPeminjaman.Count; i++)
                {
                    var p = daftarPeminjaman[i];
                    Console.WriteLine($"{i + 1}. {p.BukuDipinjam.judul} (ID: {p.BukuDipinjam.idBuku}) dipinjam oleh {p.NamaPeminjam} - Status: {p.status}");
                }
            }
            else
            {
                Console.WriteLine("=== TIDAK ADA BUKU YANG DIPINJAM ===");
            }
        }
    }
}