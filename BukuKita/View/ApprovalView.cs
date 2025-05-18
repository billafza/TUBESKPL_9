using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;
using BukuKita.Model;

namespace BukuKita.View
{
    class ApprovalView
    {
        // Method untuk menampilkan menu approval
        public void DisplayMenu(List<Approval> daftarApproval, List<Buku> daftarBuku, Admin admin)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n=== MENU KELOLA APPROVAL ===");
                Console.WriteLine("1. Lihat Semua Approval");
                Console.WriteLine("2. Lihat Approval Pending");
                Console.WriteLine("3. Approve Peminjaman");
                Console.WriteLine("4. Reject Peminjaman");
                Console.WriteLine("0. Kembali");
                Console.Write("Pilih: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        ShowAllApprovals(daftarApproval);
                        break;
                    case "2":
                        ShowPendingApprovals(daftarApproval);
                        break;
                    case "3":
                        ApproveRequest(daftarApproval, daftarBuku, admin);
                        break;
                    case "4":
                        RejectRequest(daftarApproval, admin);
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid!");
                        break;
                }
            }
        }

        // Method untuk menampilkan semua approval
        private void ShowAllApprovals(List<Approval> daftarApproval)
        {
            if (daftarApproval.Count == 0)
            {
                Console.WriteLine("Tidak ada data approval.");
                return;
            }

            Console.WriteLine("\n=== DAFTAR SEMUA APPROVAL ===");
            foreach (var approval in daftarApproval)
            {
                approval.DisplayInfo();
                Console.WriteLine("-----------------------------");
            }
        }

        // Method untuk menampilkan approval pending
        private void ShowPendingApprovals(List<Approval> daftarApproval)
        {
            var pendingApprovals = daftarApproval.Where(a => a.status == "Pending").ToList();

            if (pendingApprovals.Count == 0)
            {
                Console.WriteLine("Tidak ada approval yang menunggu persetujuan.");
                return;
            }

            Console.WriteLine("\n=== DAFTAR APPROVAL PENDING ===");
            foreach (var approval in pendingApprovals)
            {
                approval.DisplayInfo();
                Console.WriteLine("-----------------------------");
            }
        }

        // Method untuk approve peminjaman
        private void ApproveRequest(List<Approval> daftarApproval, List<Buku> daftarBuku, Admin admin)
        {
            var pendingApprovals = daftarApproval.Where(a => a.status == "Pending").ToList();

            if (pendingApprovals.Count == 0)
            {
                Console.WriteLine("Tidak ada approval yang menunggu persetujuan.");
                return;
            }

            Console.WriteLine("\n=== APPROVAL PENDING ===");
            for (int i = 0; i < pendingApprovals.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ID: {pendingApprovals[i].idApproval}, Buku: {pendingApprovals[i].judulBuku}, Peminjam: {pendingApprovals[i].namaPeminjam}");
            }

            Console.Write("\nMasukkan nomor approval yang akan diapprove: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= pendingApprovals.Count)
            {
                var approval = pendingApprovals[index - 1];

                Console.Write("Masukkan keterangan (opsional): ");
                string keterangan = Console.ReadLine();

                // Update status approval
                approval.status = "Approved";
                if (!string.IsNullOrEmpty(keterangan))
                {
                    approval.keterangan = keterangan;
                }

                // Update status buku
                var buku = daftarBuku.FirstOrDefault(b => b.idBuku == approval.idBuku);
                if (buku != null)
                {
                    buku.Borrower = approval.namaPeminjam;
                    buku.BorrowedAt = DateTime.Now;
                    Console.WriteLine($"Buku '{buku.judul}' berhasil dipinjamkan kepada {approval.namaPeminjam}.");
                }

                Console.WriteLine("Approval berhasil disetujui!");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }

        // Method untuk reject peminjaman
        private void RejectRequest(List<Approval> daftarApproval, Admin admin)
        {
            var pendingApprovals = daftarApproval.Where(a => a.status == "Pending").ToList();

            if (pendingApprovals.Count == 0)
            {
                Console.WriteLine("Tidak ada approval yang menunggu persetujuan.");
                return;
            }

            Console.WriteLine("\n=== APPROVAL PENDING ===");
            for (int i = 0; i < pendingApprovals.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ID: {pendingApprovals[i].idApproval}, Buku: {pendingApprovals[i].judulBuku}, Peminjam: {pendingApprovals[i].namaPeminjam}");
            }

            Console.Write("\nMasukkan nomor approval yang akan direject: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= pendingApprovals.Count)
            {
                var approval = pendingApprovals[index - 1];

                Console.Write("Masukkan alasan penolakan: ");
                string alasan = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(alasan))
                {
                    Console.WriteLine("Alasan penolakan tidak boleh kosong!");
                    return;
                }

                // Update status approval
                approval.status = "Rejected";
                approval.keterangan = alasan;

                Console.WriteLine("Approval berhasil ditolak!");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }
    }
}