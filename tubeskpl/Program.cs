using System;
using System.Collections.Generic;

namespace TUBESKPL_9
{
    class Program
    {
        static List<ApprovalRequest> approvalList = new List<ApprovalRequest>();

        static void Main(string[] args)
        {
            Console.Title = "Buku Kita - Admin Console";

            // Dummy Data
            approvalList.Add(new ApprovalRequest("Khansa", "C# Fundamentals", "2025-04-20", "Menunggu"));
            approvalList.Add(new ApprovalRequest("Nadya", "Database Dasar", "2025-04-22", "Menunggu"));

            MainMenu("admin", "Alwin");
        }

        static void MainMenu(string role, string username)
        {
            Console.Clear();
            Console.WriteLine($"Selamat datang, {username} (Role: {role})\n");

            if (role == "admin")
            {
                Console.WriteLine("Menu:");
                Console.WriteLine("1. Lihat & Approve Peminjaman");
                Console.WriteLine("2. Keluar");

                Console.Write("\nPilih menu: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ApprovalMenu();
                        break;
                    case "2":
                        Console.WriteLine("Keluar dari sistem...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Input tidak valid.");
                        Console.ReadKey();
                        MainMenu(role, username);
                        break;
                }
            }
        }

        static void ApprovalMenu()
        {
            Console.Clear();
            Console.WriteLine("📚 Daftar Permintaan Peminjaman:\n");

            for (int i = 0; i < approvalList.Count; i++)
            {
                var item = approvalList[i];
                Console.WriteLine($"{i + 1}. {item.NamaUser} - \"{item.JudulBuku}\" - {item.Tanggal} [Status: {item.Status}]");
            }

            Console.WriteLine("\nMasukkan nomor yang ingin diproses (0 untuk kembali): ");
            if (int.TryParse(Console.ReadLine(), out int index))
            {
                if (index == 0)
                {
                    MainMenu("admin", "Alwin");
                    return;
                }

                if (index < 1 || index > approvalList.Count)
                {
                    Console.WriteLine("Nomor tidak valid.");
                }
                else
                {
                    var selected = approvalList[index - 1];
                    Console.WriteLine($"\nPilih aksi untuk \"{selected.JudulBuku}\" oleh {selected.NamaUser}:");
                    Console.WriteLine("1. ACC");
                    Console.WriteLine("2. Tolak");
                    Console.Write("Pilih: ");
                    string aksi = Console.ReadLine();

                    if (aksi == "1") selected.Status = "Disetujui";
                    else if (aksi == "2") selected.Status = "Ditolak";
                    else Console.WriteLine("Input tidak dikenali.");

                    Console.WriteLine("\nStatus berhasil diperbarui!");
                }
            }
            else
            {
                Console.WriteLine("Input tidak valid.");
            }

            Console.WriteLine("\nTekan tombol apa saja untuk kembali ke menu approval...");
            Console.ReadKey();
            ApprovalMenu();
        }
    }

    class ApprovalRequest
    {
        public string NamaUser { get; set; }
        public string JudulBuku { get; set; }
        public string Tanggal { get; set; }
        public string Status { get; set; }

        public ApprovalRequest(string namaUser, string judulBuku, string tanggal, string status)
        {
            NamaUser = namaUser;
            JudulBuku = judulBuku;
            Tanggal = tanggal;
            Status = status;
        }
    }
}
