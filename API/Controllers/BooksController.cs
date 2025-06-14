using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Books")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BooksController(BookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all books")]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            try
            {
                var books = _bookService.GetAllBooks();
                return Ok(new { success = true, data = books, count = books.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get book by ID")]
        public ActionResult<Book> GetBookById(string id)
        {
            try
            {
                var book = _bookService.GetBookById(id);
                if (book == null)
                    return NotFound(new { success = false, message = "Book not found" });

                return Ok(new { success = true, data = book });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("available")]
        [SwaggerOperation(Summary = "Get available books")]
        public ActionResult<IEnumerable<Book>> GetAvailableBooks()
        {
            try
            {
                var books = _bookService.GetAvailableBooks();
                return Ok(new { success = true, data = books, count = books.Count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search books")]
        public ActionResult<IEnumerable<Book>> SearchBooks([FromQuery] string keyword)
        {
            try
            {
                var books = _bookService.SearchBooks(keyword ?? "");
                return Ok(new { success = true, data = books, count = books.Count, keyword });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}