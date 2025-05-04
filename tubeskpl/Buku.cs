using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tubeskpl
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

        public void DaftarBuku(List<Buku> book)
        {
            Console.WriteLine("\n=== Katalog Buku ===");
            foreach (var buku in book)
            {
                Library.displayBuku(buku);
            }
        }

        public void TambahBuku(List<Buku> book)
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

            book.Add(new Buku {idBuku= id, judul = judul, penulis= penulis, kategori = kategori, tahunTerbit = tahunTerbit });
        }

        public void HapusBuku(List<Buku> book)
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
    }
}
