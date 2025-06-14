using API.Models;
using API.Repositories;

namespace API.Services
{
    public class BookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public List<Book> GetAllBooks()
        {
            return _bookRepository.GetAll();
        }

        public Book? GetBookById(string id)
        {
            return _bookRepository.GetById(id);
        }

        public List<Book> GetAvailableBooks()
        {
            return _bookRepository.GetAvailable();
        }

        public List<Book> SearchBooks(string keyword)
        {
            return _bookRepository.Search(keyword);
        }
    }
}