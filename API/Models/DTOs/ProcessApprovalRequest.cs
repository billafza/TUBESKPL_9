using System.ComponentModel.DataAnnotations;

namespace API.Models.DTOs
{
    public class ProcessApprovalRequest
    {
        [Required(ErrorMessage = "Status harus diisi")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status harus 'Approved' atau 'Rejected'")]
        public string Status { get; set; } = "";

        public string Keterangan { get; set; } = "";
    }
}