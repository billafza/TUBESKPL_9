using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    public class CreateApprovalRequest
    {
        [Required(ErrorMessage = "ID Buku harus diisi")]
        public string IdBuku { get; set; } = "";

        [Required(ErrorMessage = "Nama peminjam harus diisi")]
        public string NamaPeminjam { get; set; } = "";
    }
}