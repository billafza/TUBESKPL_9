using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using static BookLibrary.BookLib;
using BukuKita.Model;

namespace BukuKita.View
{
    /// <summary>
    /// Kelas ApprovalView yang mengelola fungsionalitas UI terkait approval peminjaman
    /// </summary>
    class ApprovalView
    {
        /// <summary>
        /// Invariant kelas:
        /// - Semua operasi approval harus menjaga konsistensi dalam daftar approval
        /// - Status approval harus selalu salah satu dari: "Pending", "Approved", atau "Rejected"
        /// </summary>
        
        /// <summary>
        /// Menampilkan menu pengelolaan approval
        /// </summary>
        /// <param name="daftarApproval">Daftar approval yang tidak boleh null</param>
        /// <param name="daftarBuku">Daftar buku yang tidak boleh null</param>
        /// <param name="admin">Pengguna admin yang tidak boleh null</param>
        public void DisplayMenu(List<Approval> daftarApproval, List<Buku> daftarBuku, Admin admin)
        {
            // Prekondisi
            Debug.Assert(daftarApproval != null, "daftarApproval tidak boleh null");
            Debug.Assert(daftarBuku != null, "daftarBuku tidak boleh null");
            Debug.Assert(admin != null, "admin tidak boleh null");
            
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
            
            // Postkondisi: Tidak ada nilai return karena ini adalah metode UI
        }

        /// <summary>
        /// Menampilkan semua approval yang ada dalam sistem
        /// </summary>
        /// <param name="daftarApproval">Daftar approval yang tidak boleh null</param>
        private void ShowAllApprovals(List<Approval> daftarApproval)
        {
            // Prekondisi
            Debug.Assert(daftarApproval != null, "daftarApproval tidak boleh null");
            
            if (daftarApproval.Count == 0)
            {
                Console.WriteLine("Tidak ada data approval.");
                return;
            }

            Console.WriteLine("\n=== DAFTAR SEMUA APPROVAL ===");
            foreach (var approval in daftarApproval)
            {
                // Invariant: Setiap entri approval harus memiliki ID, status, dan properti yang valid
                Debug.Assert(!string.IsNullOrEmpty(approval.idApproval), "ID Approval tidak boleh null atau kosong");
                Debug.Assert(!string.IsNullOrEmpty(approval.status), "Status approval tidak boleh null atau kosong");
                
                approval.DisplayInfo();
                Console.WriteLine("-----------------------------");
            }
            
            // Postkondisi: Semua approval telah ditampilkan
        }

        /// <summary>
        /// Menampilkan hanya approval yang statusnya pending
        /// </summary>
        /// <param name="daftarApproval">Daftar approval yang tidak boleh null</param>
        private void ShowPendingApprovals(List<Approval> daftarApproval)
        {
            // Prekondisi
            Debug.Assert(daftarApproval != null, "daftarApproval tidak boleh null");
            
            var pendingApprovals = daftarApproval.Where(a => a.status == "Pending").ToList();

            if (pendingApprovals.Count == 0)
            {
                Console.WriteLine("Tidak ada approval yang menunggu persetujuan.");
                return;
            }

            Console.WriteLine("\n=== DAFTAR APPROVAL PENDING ===");
            foreach (var approval in pendingApprovals)
            {
                // Invariant: Setiap approval harus dalam status pending
                Debug.Assert(approval.status == "Pending", "Hanya approval pending yang boleh ditampilkan");
                
                approval.DisplayInfo();
                Console.WriteLine("-----------------------------");
            }
            
            // Postkondisi: Semua approval pending telah ditampilkan
        }

        /// <summary>
        /// Menyetujui permintaan peminjaman yang pending
        /// </summary>
        /// <param name="daftarApproval">Daftar approval yang tidak boleh null</param>
        /// <param name="daftarBuku">Daftar buku yang tidak boleh null</param>
        /// <param name="admin">Pengguna admin yang tidak boleh null</param>
        private void ApproveRequest(List<Approval> daftarApproval, List<Buku> daftarBuku, Admin admin)
        {
            // Prekondisi
            Debug.Assert(daftarApproval != null, "daftarApproval tidak boleh null");
            Debug.Assert(daftarBuku != null, "daftarBuku tidak boleh null");
            Debug.Assert(admin != null, "admin tidak boleh null");
            
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
                
                // Invariant: Approval yang dipilih harus dalam status pending
                Debug.Assert(approval.status == "Pending", "Hanya dapat menyetujui permintaan yang pending");

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
                    // Invariant: Buku harus tersedia untuk dipinjam
                    Debug.Assert(string.IsNullOrEmpty(buku.Borrower), "Buku harus tersedia untuk dipinjam");
                    
                    buku.Borrower = approval.namaPeminjam;
                    buku.BorrowedAt = DateTime.Now;
                    Console.WriteLine($"Buku '{buku.judul}' berhasil dipinjamkan kepada {approval.namaPeminjam}.");
                }

                Console.WriteLine("Approval berhasil disetujui!");
                
                // Postkondisi
                Debug.Assert(approval.status == "Approved", "Status approval harus diubah menjadi Approved");
                if (buku != null)
                {
                    Debug.Assert(buku.Borrower == approval.namaPeminjam, "Peminjam buku harus sesuai dengan pemohon approval");
                    Debug.Assert(buku.BorrowedAt?.Date == DateTime.Now.Date, "Tanggal peminjaman harus disetel ke tanggal sekarang");
                }
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }

        /// <summary>
        /// Menolak permintaan peminjaman yang pending
        /// </summary>
        /// <param name="daftarApproval">Daftar approval yang tidak boleh null</param>
        /// <param name="admin">Pengguna admin yang tidak boleh null</param>
        private void RejectRequest(List<Approval> daftarApproval, Admin admin)
        {
            // Prekondisi
            Debug.Assert(daftarApproval != null, "daftarApproval tidak boleh null");
            Debug.Assert(admin != null, "admin tidak boleh null");
            
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
                
                // Invariant: Approval yang dipilih harus dalam status pending
                Debug.Assert(approval.status == "Pending", "Hanya dapat menolak permintaan yang pending");

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
                
                // Postkondisi
                Debug.Assert(approval.status == "Rejected", "Status approval harus diubah menjadi Rejected");
                Debug.Assert(!string.IsNullOrEmpty(approval.keterangan), "Alasan penolakan tidak boleh kosong");
            }
            else
            {
                Console.WriteLine("Nomor tidak valid!");
            }
        }
    }
}