using System;

namespace BookLibrary
{
    public static class BookLib
    {
        public class Buku
        {
            // Properti
            public string idBuku { get; set; }
            public string judul { get; set; }
            public string penulis { get; set; }
            public string kategori { get; set; }
            public int tahunTerbit { get; set; }
            public bool IsAvailable => Borrower == null;
            public string Borrower { get; set; }
            public DateTime? BorrowedAt { get; set; }

            // Konstruktor
            public Buku() { }
            public Buku(string idBuku, string judul, string penulis, string kategori, int tahunTerbit)
            {
                this.idBuku = idBuku;
                this.judul = judul;
                this.penulis = penulis;
                this.kategori = kategori;
                this.tahunTerbit = tahunTerbit;
            }

            // Method untuk menampilkan sebuah buku
            public static void displayBuku(Buku b)
            {
                Console.WriteLine($"\nID Buku: {b.idBuku} \nJudul: {b.judul} oleh {b.penulis} \nKategori: {b.kategori} \nTahun Terbit: {b.tahunTerbit}");
            }

            // Method untuk menampilkan semua buku
            public static void DaftarBuku(List<Buku> book)
            {
                Console.WriteLine("\n=== Katalog Buku ===");
                foreach (var buku in book)
                {
                    Buku.displayBuku(buku);
                }
            }

            // Method untuk menambah buku dari inputan user
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

                Console.WriteLine($"Buku {judul} berhasil ditambahkan.");
            }

            // Method untuk menghapus buku berdasarkan ID
            public static void HapusBuku(List<Buku> book)
            {
                Console.WriteLine("Masukkan ID Buku yang ingin dihapus: ");
                string id = Console.ReadLine();

                Buku hapusBuku = book.FirstOrDefault(b => b.idBuku.Equals(id, StringComparison.OrdinalIgnoreCase));

                if (hapusBuku != null)
                {
                    book.Remove(hapusBuku);
                    Console.WriteLine($"Buku dengan ID {id} berhasil dihapus.");
                }
                else
                {
                    Console.WriteLine($"Buku dengan ID {id} tidak ditemukan.");
                }
            }

            // Method filter pencarian buku berdasarkan ID/Judul/Penulis/Kategori
            public static List<Buku> FilterBuku(List<Buku> daftarBuku, string keyword)
            {
                return daftarBuku
                    .Where(b => b.idBuku.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                             || b.judul.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                             || b.penulis.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                             || b.kategori.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Method untuk menampilkan menu kelola buku untuk Admin
            public enum State
            {
                Menu,
                LihatBuku,
                TambahBuku,
                HapusBuku,
                Keluar
            }

            public static void KelolaBuku(List<Buku> book)
            {
                State currentState = State.Menu;
                bool isRunning = true;

                while (isRunning)
                {
                    switch (currentState)
                    {
                        case State.Menu:
                            Console.WriteLine("\n=== MENU KELOLA BUKU ===");
                            Console.WriteLine("1. Melihat Buku");
                            Console.WriteLine("2. Menambah Buku");
                            Console.WriteLine("3. Menghapus Buku");
                            Console.WriteLine("0. Kembali");
                            Console.Write("Pilih: ");
                            string input = Console.ReadLine();

                            switch (input)
                            {
                                case "1":
                                    currentState = State.LihatBuku;
                                    break;
                                case "2":
                                    currentState = State.TambahBuku;
                                    break;
                                case "3":
                                    currentState = State.HapusBuku;
                                    break;
                                case "0":
                                    currentState = State.Keluar;
                                    break;
                                default:
                                    Console.WriteLine("Pilihan tidak valid.");
                                    break;
                            }
                            break;

                        case State.LihatBuku:
                            DaftarBuku(book);
                            currentState = State.Menu;
                            break;

                        case State.TambahBuku:
                            TambahBuku(book);
                            currentState = State.Menu;
                            break;

                        case State.HapusBuku:
                            HapusBuku(book);
                            currentState = State.Menu;
                            break;

                        case State.Keluar:
                            isRunning = false;
                            break;
                    }
                }
            }
        }
    }
}