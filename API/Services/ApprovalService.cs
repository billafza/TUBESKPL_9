using BukuKita;
using BukuKita.Model;

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
            var approvals = _mainMenu.GetAllApprovals();
            return approvals.FirstOrDefault(a => a.idApproval == id);
        }

        public List<Approval> GetApprovalsByUser(string userName)
        {
            var approvals = _mainMenu.GetAllApprovals();
            return approvals.Where(a => a.namaPeminjam.Equals(userName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public ApiResult<Approval> CreateApproval(string idBuku, string namaPeminjam)
        {
            var allApprovals = _mainMenu.GetAllApprovals();

            // Check if user already has pending approval for this book
            bool hasPendingApproval = allApprovals.Any(a =>
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
            var userPendingApprovals = allApprovals.Count(a =>
                a.namaPeminjam.Equals(namaPeminjam, StringComparison.OrdinalIgnoreCase) &&
                a.status == "Pending");

            if (userPendingApprovals >= maxPeminjaman)
            {
                return ApiResult<Approval>.Error($"Maksimum {maxPeminjaman} approval pending per user sudah tercapai");
            }

            // Create approval using MainMenu
            var approval = _mainMenu.CreateApproval(idBuku, buku.judul, namaPeminjam);
            return ApiResult<Approval>.Success(approval, "Approval berhasil dibuat");
        }

        public ApiResult<Approval> ProcessApproval(string id, string status, string keterangan = "")
        {
            var approval = GetApprovalById(id);
            if (approval == null)
            {
                return ApiResult<Approval>.Error("Approval tidak ditemukan");
            }

            if (approval.status != "Pending")
            {
                return ApiResult<Approval>.Error("Hanya dapat memproses approval dengan status 'Pending'");
            }

            // Use MainMenu method
            _mainMenu.ProcessApproval(approval, status, keterangan);

            string message = status == "Approved" ? "Approval berhasil disetujui" : "Approval berhasil ditolak";
            return ApiResult<Approval>.Success(approval, message);
        }

        public ApiResult<bool> DeleteApproval(string id)
        {
            var approval = GetApprovalById(id);
            if (approval == null)
            {
                return ApiResult<bool>.Error("Approval tidak ditemukan");
            }

            if (approval.status != "Pending")
            {
                return ApiResult<bool>.Error("Hanya dapat menghapus approval dengan status 'Pending'");
            }

            var allApprovals = _mainMenu.GetAllApprovals();
            bool removed = allApprovals.Remove(approval);

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

    // Simple ApiResult class
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; } = default(T)!;
        public string Message { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public static ApiResult<T> Success(T data, string message = "")
        {
            return new ApiResult<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResult<T> Error(string errorMessage)
        {
            return new ApiResult<T>
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }
    }
}