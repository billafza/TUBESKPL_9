using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    /// <summary>
    /// Request model untuk memproses approval
    /// </summary>
    public class ProcessApprovalRequest
    {
        /// <summary>
        /// Status: "Approved" atau "Rejected"
        /// </summary>
        /// <example>Approved</example>
        [Required(ErrorMessage = "Status harus diisi")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status harus 'Approved' atau 'Rejected'")]
        public string Status { get; set; } = "";

        /// <summary>
        /// Keterangan atau catatan (opsional)
        /// </summary>
        /// <example>Peminjaman disetujui</example>
        public string Keterangan { get; set; } = "";
    }
}