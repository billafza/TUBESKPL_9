using BukuKita;
using BukuKita.Model;
using API.Models;

namespace API.Services
{
    public class ApprovalService
    {
        private readonly MainMenu _mainMenu;

        public ApprovalService(MainMenu mainMenu)
        {
            _mainMenu = mainMenu;
        }

        public List<Approval> GetAllApprovals()
        {
            return _mainMenu.GetAllApprovals();
        }

        public List<Approval> GetPendingApprovals()
        {
            return _mainMenu.GetPendingApprovals();
        }

        public Approval? GetApprovalById(string id)
        {
            return _mainMenu.GetApprovalById(id);
        }

        public List<Approval> GetApprovalsByUser(string userName)
        {
            return _mainMenu.GetApprovalsByUser(userName);
        }

        public ApiResult<Approval> CreateApproval(string idBuku, string namaPeminjam)
        {
            var book = _mainMenu.GetBookById(idBuku);
            if (book == null)
                return ApiResult<Approval>.Error("Book not found");

            return CreateApprovalWithValidation(idBuku, book.judul, namaPeminjam);
        }

        public ApiResult<Approval> ProcessApproval(string id, string status, string keterangan = "")
        {
            return ProcessApprovalWithValidation(id, status, keterangan);
        }

        public ApiResult<bool> DeleteApproval(string id)
        {
            return DeleteApprovalWithValidation(id);
        }

        private ApiResult<Approval> CreateApprovalWithValidation(string idBuku, string judulBuku, string namaPeminjam)
        {
            var approvals = _mainMenu.GetAllApprovals();

            // Check if user already has pending approval for this book
            bool hasPendingApproval = approvals.Any(a =>
                a.idBuku == idBuku &&
                a.namaPeminjam.Equals(namaPeminjam, StringComparison.OrdinalIgnoreCase) &&
                a.status == "Pending");

            if (hasPendingApproval)
            {
                return ApiResult<Approval>.Error("Anda sudah memiliki approval pending untuk buku ini");
            }

            // Check if book exists and is available
            var buku = _mainMenu.GetBookById(idBuku);
            if (buku == null)
            {
                return ApiResult<Approval>.Error("Buku tidak ditemukan");
            }

            if (!string.IsNullOrEmpty(buku.Borrower))
            {
                return ApiResult<Approval>.Error("Buku sedang dipinjam oleh orang lain");
            }

            // Check maximum approvals per user
            int maxPeminjaman = _mainMenu.GetConfigValue<int>("MaxPeminjamanPerUser", 3);
            var userPendingApprovals = approvals.Count(a =>
                a.namaPeminjam.Equals(namaPeminjam, StringComparison.OrdinalIgnoreCase) &&
                a.status == "Pending");

            if (userPendingApprovals >= maxPeminjaman)
            {
                return ApiResult<Approval>.Error($"Maksimum {maxPeminjaman} approval pending per user sudah tercapai");
            }

            var approval = _mainMenu.CreateApproval(idBuku, judulBuku, namaPeminjam);
            return ApiResult<Approval>.Success(approval, "Approval berhasil dibuat");
        }

        private ApiResult<Approval> ProcessApprovalWithValidation(string idApproval, string newStatus, string keterangan = "")
        {
            var approval = GetApprovalById(idApproval);
            if (approval == null)
            {
                return ApiResult<Approval>.Error("Approval tidak ditemukan");
            }

            if (approval.status != "Pending")
            {
                return ApiResult<Approval>.Error("Hanya dapat memproses approval dengan status 'Pending'");
            }

            approval.status = newStatus;
            if (!string.IsNullOrEmpty(keterangan))
            {
                approval.keterangan = keterangan;
            }

            // Jika disetujui, perbarui status buku dan buat peminjaman
            if (newStatus == "Approved")
            {
                var buku = _mainMenu.GetBookById(approval.idBuku);
                if (buku != null)
                {
                    if (!string.IsNullOrEmpty(buku.Borrower))
                    {
                        // Rollback approval status
                        approval.status = "Pending";
                        approval.keterangan = "";
                        return ApiResult<Approval>.Error("Buku sudah dipinjam oleh orang lain");
                    }

                    // Update book status
                    buku.Borrower = approval.namaPeminjam;
                    buku.BorrowedAt = DateTime.Now;

                    // Create peminjaman record
                    var peminjaman = _mainMenu.CreatePeminjaman(buku, approval.namaPeminjam);
                }
            }

            string message = newStatus == "Approved" ? "Approval berhasil disetujui" : "Approval berhasil ditolak";
            return ApiResult<Approval>.Success(approval, message);
        }

        private ApiResult<bool> DeleteApprovalWithValidation(string idApproval)
        {
            var approval = GetApprovalById(idApproval);
            if (approval == null)
            {
                return ApiResult<bool>.Error("Approval tidak ditemukan");
            }

            if (approval.status != "Pending")
            {
                return ApiResult<bool>.Error("Hanya dapat menghapus approval dengan status 'Pending'");
            }

            var approvals = _mainMenu.GetAllApprovals();
            bool removed = approvals.Remove(approval);

            if (removed)
            {
                return ApiResult<bool>.Success(true, "Approval berhasil dihapus");
            }
            else
            {
                return ApiResult<bool>.Error("Gagal menghapus approval");
            }
        }
    }
}