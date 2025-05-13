using System;
using System.Collections.Generic;
using System.Linq;

public class Buku
{
    public string idBuku { get; set; }
    public string judul { get; set; }
    public string penulis { get; set; }
    public string kategori { get; set; }
    public int tahunTerbit { get; set; }

    public override string ToString()
    {
        return $"ID: {idBuku}, Judul: {judul}";
    }
}

public class Peminjaman
{
    public Buku BukuDipinjam { get; set; }
    public string NamaPeminjam { get; set; }
    public DateTime TanggalPinjam { get; set; }
    public bool SudahKembali { get; private set; }

    public Peminjaman(Buku buku, string nama)
    {
        BukuDipinjam = buku;
        NamaPeminjam = nama;
        TanggalPinjam = DateTime.Now;
        SudahKembali = false;
    }

    public void Kembalikan()
    {
        SudahKembali = true;
    }

    public string Info()
    {
        string status = SudahKembali ? "Sudah dikembalikan" : "Belum dikembalikan";
        return $"Judul: {BukuDipinjam.judul}, Peminjam: {NamaPeminjam}, Tanggal Pinjam: {TanggalPinjam.ToShortDateString()}, Status: {status}";
    }
}

class Program
{
    static void Main()
    {
        // Data dummy peminjaman aktif (asumsi sebelumnya dipinjam)
        var buku1 = new Buku { idBuku = "B02", judul = "Pemrograman C#", penulis = "Andi", kategori = "Komputer", tahunTerbit = 2021 };
        var peminjaman = new Peminjaman(buku1, "Adit");

        Console.WriteLine("Data Peminjaman Sebelum Pengembalian:");
        Console.WriteLine(peminjaman.Info());

        Console.Write("\nMasukkan ID buku yang ingin dikembalikan: ");
        string id = Console.ReadLine();

        if (id == peminjaman.BukuDipinjam.idBuku && !peminjaman.SudahKembali)
        {
            peminjaman.Kembalikan();
            Console.WriteLine("\n--- Bukti Pengembalian ---");
            Console.WriteLine(peminjaman.Info());
        }
        else
        {
            Console.WriteLine("\nGagal: Buku tidak ditemukan atau sudah dikembalikan.");
        }
    }
}
