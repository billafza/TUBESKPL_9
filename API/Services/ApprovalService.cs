using API.Models;
using API.Repositories;

namespace API.Services
{
    public class ApprovalService
    {
        private readonly IApprovalRepository _approvalRepository;
        private readonly IBookRepository _bookRepository;
        private readonly int _maxPeminjamanPerUser = 3;

        public ApprovalService(IApprovalRepository approvalRepository, IBookRepository bookRepository)
        {
            _approvalRepository = approvalRepository;
            _bookRepository = bookRepository;
        }

        public List<Approval> GetAllApprovals()
        {
            return _approvalRepository.GetAll();
        }

        public List<Approval> GetPendingApprovals()
        {
            return _approvalRepository.GetPending();
        }

        public Approval? GetApprovalById(string id)
        {
            return _approvalRepository.GetById(id);
        }

        public List<Approval> GetApprovalsByUser(string userName)
        {
            return _approvalRepository.GetByUser(userName);
        }

        public ApiResult<Approval> CreateApproval(string idBuku, string namaPeminjam)
        {
            // Check if user already has pending approval for this book
            var userApprovals = _approvalRepository.GetByUser(namaPeminjam);
            bool hasPendingApproval = userApprovals.Any(a =>
                a.IdBuku == idBuku && a.Status == "Pending");

            if (hasPendingApproval)
            {
                return ApiResult<Approval>.Error("Anda sudah memiliki approval pending untuk buku ini");
            }

            // Check if book exists and is available
            var book = _bookRepository.GetById(idBuku);
            if (book == null)
            {
                return ApiResult<Approval>.Error("Buku tidak ditemukan");
            }

            if (!book.IsAvailable)
            {
                return ApiResult<Approval>.Error("Buku sedang dipinjam oleh orang lain");
            }

            // Check maximum approvals per user
            var userPendingApprovals = userApprovals.Count(a => a.Status == "Pending");
            if (userPendingApprovals >= _maxPeminjamanPerUser)
            {
                return ApiResult<Approval>.Error($"Maksimum {_maxPeminjamanPerUser} approval pending per user sudah tercapai");
            }

            // Create approval
            var approval = new Approval
            {
                IdApproval = _approvalRepository.GenerateNewId(),
                IdBuku = idBuku,
                JudulBuku = book.Judul,
                NamaPeminjam = namaPeminjam,
                TanggalPengajuan = DateTime.Now,
                Status = "Pending",
                Keterangan = ""
            };

            _approvalRepository.Add(approval);
            return ApiResult<Approval>.Success(approval, "Approval berhasil dibuat");
        }

        public ApiResult<Approval> ProcessApproval(string id, string status, string keterangan = "")
        {
            var approval = _approvalRepository.GetById(id);
            if (approval == null)
            {
                return ApiResult<Approval>.Error("Approval tidak ditemukan");
            }

            if (approval.Status != "Pending")
            {
                return ApiResult<Approval>.Error("Hanya dapat memproses approval dengan status 'Pending'");
            }

            // If approved, check if book is still available
            if (status == "Approved")
            {
                var book = _bookRepository.GetById(approval.IdBuku);
                if (book != null && !book.IsAvailable)
                {
                    return ApiResult<Approval>.Error("Buku sudah dipinjam oleh orang lain");
                }

                // Update book borrower
                _bookRepository.UpdateBorrower(approval.IdBuku, approval.NamaPeminjam);
            }

            // Update approval
            approval.UpdateStatus(status, keterangan);
            _approvalRepository.Update(approval);

            string message = status == "Approved" ? "Approval berhasil disetujui" : "Approval berhasil ditolak";
            return ApiResult<Approval>.Success(approval, message);
        }

        public ApiResult<bool> DeleteApproval(string id)
        {
            var approval = _approvalRepository.GetById(id);
            if (approval == null)
            {
                return ApiResult<bool>.Error("Approval tidak ditemukan");
            }

            if (approval.Status != "Pending")
            {
                return ApiResult<bool>.Error("Hanya dapat menghapus approval dengan status 'Pending'");
            }

            bool removed = _approvalRepository.Delete(id);
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