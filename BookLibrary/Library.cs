using System;

namespace BookLibrary
{
    public static class Library
    {
        public class Buku
        {
            public string idBuku { get; set; }
            public string judul { get; set; }
            public string penulis { get; set; }
            public string kategori { get; set; }
            public int tahunTerbit { get; set; }

            public Buku() { }
            public Buku(string idBuku, string judul, string penulis, string kategori, int tahunTerbit)
            {
                this.idBuku = idBuku;
                this.judul = judul;
                this.penulis = penulis;
                this.kategori = kategori;
                this.tahunTerbit = tahunTerbit;
            }

            public static void displayBuku(Buku b)
            {
                Console.WriteLine($"\nID Buku: {b.idBuku} \nJudul: {b.judul} oleh {b.penulis} \nKategori: {b.kategori} \nTahun Terbit: {b.tahunTerbit}");
            }

            public static void DaftarBuku(List<Buku> book)
            {
                Console.WriteLine("\n=== Katalog Buku ===");
                foreach (var buku in book)
                {
                    Buku.displayBuku(buku);
                }
            }

            public static void TambahBuku(List<Buku> book)
            {
                Console.WriteLine("ID Buku: ");
                string id = Console.ReadLine();

                Console.WriteLine("Judul: ");
                string judul = Console.ReadLine();

                Console.WriteLine("Penulis: ");
                string penulis = Console.ReadLine();

                Console.WriteLine("Kategori: ");
                string kategori = Console.ReadLine();

                Console.WriteLine("Tahun Terbit: ");
                int tahunTerbit = int.Parse(Console.ReadLine());

                book.Add(new Buku { idBuku = id, judul = judul, penulis = penulis, kategori = kategori, tahunTerbit = tahunTerbit });
            }

            public static void HapusBuku(List<Buku> book)
            {

            }
            public static List<Buku> FilterBuku(List<Buku> daftarBuku, string keyword)
            {
                return daftarBuku
                    .Where(b => b.idBuku.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                             || b.judul.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                             || b.penulis.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                             || b.kategori.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            public static void KelolaBuku(List<Buku> book)
            {
                bool isRunning = true;
                while (isRunning)
                {

                    Console.WriteLine("\n=== MENU KELOLA BUKU ===");
                    Console.WriteLine("1. Melihat Buku");
                    Console.WriteLine("2. Menambah Buku");
                    Console.WriteLine("3. Menghapus Buku");
                    Console.WriteLine("0. Kembali");
                    int pilih = int.Parse(Console.ReadLine());
                    Console.WriteLine();

                    switch (pilih)
                    {
                        case 1:
                            DaftarBuku(book);
                            break;

                        case 2:
                            TambahBuku(book);
                            break;

                        case 3:
                            HapusBuku(book);
                            break;

                        case 0:
                            isRunning = false;
                            break;

                        default:
                            Console.WriteLine("\nPilih menu yang sesuai.");
                            break;
                    }
                }
            }
        }
    }
}