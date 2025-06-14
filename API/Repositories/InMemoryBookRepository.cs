using API.Models;

namespace API.Repositories
{
    public class InMemoryBookRepository : IBookRepository
    {
        private static readonly List<Book> _books = InitializeBooks();

        private static List<Book> InitializeBooks()
        {
            return new List<Book>
            {
                new Book { IdBuku = "B01", Judul = "Kancil Cerdik", Kategori = "Dongeng", Penulis = "Cecylia", TahunTerbit = 2010 },
                new Book { IdBuku = "B02", Judul = "Artificial Intelligence", Kategori = "Teknik Komputer", Penulis = "Budiono", TahunTerbit = 2015 },
                new Book { IdBuku = "B03", Judul = "Bunga Sayu", Kategori = "Novel", Penulis = "Suci Ratna", TahunTerbit = 2020 },
                new Book { IdBuku = "B04", Judul = "Sejarah Indonesia", Kategori = "Sejarah", Penulis = "Sri Handayani", TahunTerbit = 2021 },
                new Book { IdBuku = "B05", Judul = "Sejarah Umum Indonesia", Kategori = "Sejarah", Penulis = "Sri Suryani", TahunTerbit = 2011 }
            };
        }

        public List<Book> GetAll()
        {
            return _books.ToList();
        }

        public Book? GetById(string id)
        {
            return _books.FirstOrDefault(b => b.IdBuku == id);
        }

        public List<Book> GetAvailable()
        {
            return _books.Where(b => b.IsAvailable).ToList();
        }

        public List<Book> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<Book>();

            return _books.Where(b =>
                b.Judul.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.Penulis.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                b.Kategori.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public void UpdateBorrower(string bookId, string? borrower)
        {
            var book = GetById(bookId);
            if (book != null)
            {
                book.Borrower = borrower;
                book.BorrowedAt = string.IsNullOrEmpty(borrower) ? null : DateTime.Now;
            }
        }
    }
}