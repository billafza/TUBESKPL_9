using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookLibrary.BookLib;

namespace BukuKita.Model
{
    public class Admin : User
    {
        public Admin(string nama, string nim, EnumJenisKelamin jnsKelamin, string email, string password, string role)
            : base(nama, jnsKelamin, email, password, role)
        {
            this.NIM = nim;
        }

        public static void KelolaPengguna(List<Peminjaman> daftarPeminjaman, List<Approval> daftarApproval, List<Buku> daftarBuku)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n=== MENU KELOLA PENGGUNA ===");
                Console.WriteLine("1. Lihat Data Peminjaman");
                Console.WriteLine("2. Kelola Approval Peminjaman");
                Console.WriteLine("0. Kembali");
                Console.Write("Pilih: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        Peminjaman.ShowDataPeminjam(daftarPeminjaman);
                        break;
                    case "2":
                        KelolaApproval(daftarApproval, daftarPeminjaman, daftarBuku);
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }
            }
        }

        public static void KelolaApproval(List<Approval> daftarApproval, List<Peminjaman> daftarPeminjaman, List<Buku> daftarBuku)
        {
            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("\n=== MENU KELOLA APPROVAL ===");
                Console.WriteLine("1. Lihat Semua Approval");
                Console.WriteLine("2. Approve Peminjaman");
                Console.WriteLine("3. Reject Peminjaman");
                Console.WriteLine("0. Kembali");
                Console.Write("Pilih: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        TampilkanDaftarApproval(daftarApproval);
                        break;
                    case "2":
                        ApprovePeminjaman(daftarApproval, daftarPeminjaman, daftarBuku);
                        break;
                    case "3":
                        RejectPeminjaman(daftarApproval, daftarPeminjaman);
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }
            }
        }

        private static void TampilkanDaftarApproval(List<Approval> daftarApproval)
        {
            if (daftarApproval.Count == 0)
            {
                Console.WriteLine("Tidak ada data approval.");
                return;
            }

            Console.WriteLine("\n=== DAFTAR APPROVAL ===");
            for (int i = 0; i < daftarApproval.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ");
                daftarApproval[i].DisplayInfo();
                Console.WriteLine("----------------------------");
            }
        }

        private static void ApprovePeminjaman(List<Approval> daftarApproval, List<Peminjaman> daftarPeminjaman, List<Buku> daftarBuku)
        {
            // Filter approval yang masih pending
            var pendingApprovals = daftarApproval.Where(a => a.status == "Pending").ToList();
            if (pendingApprovals.Count == 0)
            {
                Console.WriteLine("Tidak ada approval yang perlu diproses.");
                return;
            }

            Console.WriteLine("\n=== APPROVAL PENDING ===");
            for (int i = 0; i < pendingApprovals.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ID: {pendingApprovals[i].idApproval}, Buku: {pendingApprovals[i].judulBuku}, Peminjam: {pendingApprovals[i].namaPeminjam}");
            }

            Console.Write("\nPilih nomor approval yang akan diapprove: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= pendingApprovals.Count)
            {
                var approval = pendingApprovals[index - 1];
                approval.status = "Approved";

                Console.Write("Tambahkan keterangan (opsional): ");
                string keterangan = Console.ReadLine();
                if (!string.IsNullOrEmpty(keterangan))
                {
                    approval.keterangan = keterangan;
                }

                // Update status peminjaman terkait
                var peminjaman = daftarPeminjaman.FirstOrDefault(p =>
                    p.BukuDipinjam.idBuku == approval.idBuku &&
                    p.NamaPeminjam == approval.namaPeminjam &&
                    p.status == "Pending");

                if (peminjaman != null)
                {
                    peminjaman.status = "Approved";

                    // Update status buku
                    var buku = daftarBuku.FirstOrDefault(b => b.idBuku == approval.idBuku);
                    if (buku != null)
                    {
                        buku.Borrower = approval.namaPeminjam;
                        buku.BorrowedAt = DateTime.Now;
                    }
                }

                Console.WriteLine("Peminjaman berhasil diapprove!");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }

        private static void RejectPeminjaman(List<Approval> daftarApproval, List<Peminjaman> daftarPeminjaman)
        {
            // Filter approval yang masih pending
            var pendingApprovals = daftarApproval.Where(a => a.status == "Pending").ToList();
            if (pendingApprovals.Count == 0)
            {
                Console.WriteLine("Tidak ada approval yang perlu diproses.");
                return;
            }

            Console.WriteLine("\n=== APPROVAL PENDING ===");
            for (int i = 0; i < pendingApprovals.Count; i++)
            {
                Console.WriteLine($"{i + 1}. ID: {pendingApprovals[i].idApproval}, Buku: {pendingApprovals[i].judulBuku}, Peminjam: {pendingApprovals[i].namaPeminjam}");
            }

            Console.Write("\nPilih nomor approval yang akan direject: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= pendingApprovals.Count)
            {
                var approval = pendingApprovals[index - 1];

                Console.Write("Masukkan alasan penolakan: ");
                string alasan = Console.ReadLine();
                if (string.IsNullOrEmpty(alasan))
                {
                    Console.WriteLine("Alasan penolakan tidak boleh kosong!");
                    return;
                }

                approval.status = "Rejected";
                approval.keterangan = alasan;

                // Update status peminjaman terkait
                var peminjaman = daftarPeminjaman.FirstOrDefault(p =>
                    p.BukuDipinjam.idBuku == approval.idBuku &&
                    p.NamaPeminjam == approval.namaPeminjam &&
                    p.status == "Pending");

                if (peminjaman != null)
                {
                    peminjaman.status = "Rejected";
                }

                Console.WriteLine("Peminjaman berhasil direject!");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }
    }
}