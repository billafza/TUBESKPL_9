using API.Models;

namespace API.Repositories
{
    public interface IBookRepository
    {
        List<Book> GetAll();
        Book? GetById(string id);
        List<Book> GetAvailable();
        List<Book> Search(string keyword);
        void UpdateBorrower(string bookId, string? borrower);
    }
}