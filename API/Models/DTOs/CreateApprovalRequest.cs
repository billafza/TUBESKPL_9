using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    /// <summary>
    /// Request model untuk membuat approval baru
    /// </summary>
    public class CreateApprovalRequest
    {
        /// <summary>
        /// ID buku yang akan dipinjam
        /// </summary>
        /// <example>B01</example>
        [Required(ErrorMessage = "ID Buku harus diisi")]
        public string IdBuku { get; set; } = "";

        /// <summary>
        /// Nama peminjam
        /// </summary>
        /// <example>John Doe</example>
        [Required(ErrorMessage = "Nama peminjam harus diisi")]
        public string NamaPeminjam { get; set; } = "";
    }
}