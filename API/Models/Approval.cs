namespace API.Models
{
    public class Approval
    {
        public string IdApproval { get; set; } = "";
        public string IdBuku { get; set; } = "";
        public string JudulBuku { get; set; } = "";
        public string NamaPeminjam { get; set; } = "";
        public DateTime TanggalPengajuan { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"
        public string Keterangan { get; set; } = "";

        public void UpdateStatus(string newStatus, string keterangan = "")
        {
            Status = newStatus;
            if (!string.IsNullOrEmpty(keterangan))
            {
                Keterangan = keterangan;
            }
        }
    }
}