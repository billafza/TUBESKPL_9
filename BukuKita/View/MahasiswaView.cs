using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
using BukuKita.Model;

namespace BukuKita.View
{
    class MahasiswaView
    {
        public void DisplayMenu(List<Buku> book, List<Peminjaman> daftarPeminjaman, List<Pengembalian> daftarPengembalian, List<Approval> daftarApproval)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n=== MENU MAHASISWA ===");
                Console.WriteLine("1. Lihat Data Buku");
                Console.WriteLine("2. Pinjam Buku");
                Console.WriteLine("3. Lihat Status Peminjaman");
                Console.WriteLine("4. Kembalikan Buku");
                Console.WriteLine("0. Logout");
                Console.Write("Pilih: ");
                int input = int.Parse(Console.ReadLine());
                Console.WriteLine();
                switch (input)
                {
                    case 1:
                        Buku.DaftarBuku(book); // Sesuaikan dengan metode yang ada di Buku
                        break;
                    case 2:
                        PinjamBuku(book, daftarPeminjaman, daftarApproval);
                        break;
                    case 3:
                        LihatStatusPeminjaman(daftarPeminjaman, daftarApproval);
                        break;
                    case 4:
                        PengembalianView.KembalikanBuku(book, daftarPeminjaman, daftarPengembalian);
                        break;
                    case 0:
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("\nPilih menu yang sesuai [1-4] / 0 untuk keluar.");
                        break;
                }
            }
        }

        private void PinjamBuku(List<Buku> book, List<Peminjaman> daftarPeminjaman, List<Approval> daftarApproval)
        {
            if (book.Count == 0)
            {
                Console.WriteLine("Tidak ada data buku.");
                return;
            }

            Console.WriteLine("=== DAFTAR BUKU ===");
            for (int i = 0; i < book.Count; i++)
            {
                var b = book[i];
                string status = string.IsNullOrEmpty(b.Borrower) ? "Tersedia" : "Dipinjam";
                Console.WriteLine($"{i + 1}. [{b.idBuku}] {b.judul} ({status})");
            }

            Console.Write("\nMasukkan ID buku yang ingin dipinjam: ");
            string id = Console.ReadLine();

            var bukuDipinjam = book.FirstOrDefault(b => b.idBuku == id);
            if (bukuDipinjam == null)
            {
                Console.WriteLine("ID buku tidak ditemukan.");
                return;
            }

            if (!string.IsNullOrEmpty(bukuDipinjam.Borrower))
            {
                Console.WriteLine($"Buku '{bukuDipinjam.judul}' sedang dipinjam oleh {bukuDipinjam.Borrower}.");
                return;
            }

            Console.Write("Masukkan nama peminjam: ");
            string namaPeminjam = Console.ReadLine();

            // Buat peminjaman baru
            var peminjaman = new Peminjaman(bukuDipinjam, namaPeminjam);
            daftarPeminjaman.Add(peminjaman);

            // Buat approval baru
            string idApproval = $"APV{daftarApproval.Count + 1:D3}";
            var approval = new Approval(idApproval, bukuDipinjam.idBuku, bukuDipinjam.judul, namaPeminjam);
            daftarApproval.Add(approval);

            Console.WriteLine($"\nPengajuan peminjaman buku '{bukuDipinjam.judul}' berhasil dibuat!");
            Console.WriteLine("Status: Menunggu approval dari admin.");
            Console.WriteLine($"ID Approval: {idApproval}");
        }

        private void LihatStatusPeminjaman(List<Peminjaman> daftarPeminjaman, List<Approval> daftarApproval)
        {
            Console.Write("Masukkan nama peminjam: ");
            string namaPeminjam = Console.ReadLine();

            var peminjaman = daftarPeminjaman.Where(p => p.NamaPeminjam == namaPeminjam).ToList();
            if (peminjaman.Count == 0)
            {
                Console.WriteLine($"Tidak ada data peminjaman untuk {namaPeminjam}.");
                return;
            }

            Console.WriteLine($"\n=== STATUS PEMINJAMAN {namaPeminjam.ToUpper()} ===");
            foreach (var p in peminjaman)
            {
                Console.WriteLine($"Judul: {p.BukuDipinjam.judul}");
                Console.WriteLine($"ID Buku: {p.BukuDipinjam.idBuku}");
                Console.WriteLine($"Tanggal Pinjam: {p.TanggalPinjam.ToString("dd/MM/yyyy HH:mm")}");
                Console.WriteLine($"Status: {p.status}");

                // Cari approval terkait
                var approval = daftarApproval.FirstOrDefault(a =>
                    a.idBuku == p.BukuDipinjam.idBuku &&
                    a.namaPeminjam == p.NamaPeminjam);

                if (approval != null)
                {
                    Console.WriteLine($"ID Approval: {approval.idApproval}");
                    Console.WriteLine($"Status Approval: {approval.status}");
                    if (!string.IsNullOrEmpty(approval.keterangan))
                    {
                        Console.WriteLine($"Keterangan: {approval.keterangan}");
                    }
                }

                Console.WriteLine("----------------------------");
            }
        }
    }
}