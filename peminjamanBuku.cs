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
        return $"ID: {idBuku}, Judul: {judul}, Penulis: {penulis}, Kategori: {kategori}, Tahun: {tahunTerbit}";
    }
}

public class Peminjaman
{
    public Buku BukuDipinjam { get; set; }
    public string NamaPeminjam { get; set; }
    public DateTime TanggalPinjam { get; set; }

    public Peminjaman(Buku buku, string nama)
    {
        BukuDipinjam = buku;
        NamaPeminjam = nama;
        TanggalPinjam = DateTime.Now;
    }

    public string Info()
    {
        return $"Judul: {BukuDipinjam.judul}, Dipinjam oleh: {NamaPeminjam}, Tanggal: {TanggalPinjam.ToShortDateString()}";
    }
}

class Program
{
    static void Main()
    {
        List<Buku> daftarBuku = new List<Buku>
        {
            new Buku { idBuku = "B01", judul = "Algoritma", penulis = "Dian", kategori = "Teknik", tahunTerbit = 2019 },
            new Buku { idBuku = "B02", judul = "Pemrograman C#", penulis = "Andi", kategori = "Komputer", tahunTerbit = 2021 },
            new Buku { idBuku = "B03", judul = "Sejarah Dunia", penulis = "Rini", kategori = "Sejarah", tahunTerbit = 2018 }
        };

        List<Peminjaman> daftarPeminjaman = new List<Peminjaman>();

        Console.WriteLine("Daftar Buku:");
        foreach (var buku in daftarBuku)
        {
            Console.WriteLine(buku);
        }

        Console.Write("\nMasukkan ID buku yang ingin dipinjam: ");
        string id = Console.ReadLine();
        Buku bukuDipilih = daftarBuku.FirstOrDefault(b => b.idBuku == id);

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