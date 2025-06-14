namespace API.Models
{
    public class Book
    {
        public string IdBuku { get; set; } = "";
        public string Judul { get; set; } = "";
        public string Penulis { get; set; } = "";
        public string Kategori { get; set; } = "";
        public int TahunTerbit { get; set; }
        public string? Borrower { get; set; }
        public DateTime? BorrowedAt { get; set; }

        public bool IsAvailable => string.IsNullOrEmpty(Borrower);
    }
}