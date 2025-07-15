using LibraryManagement.Domain.Dtos;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Repositories;
using LibraryManagement.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemoryCache _cache;
    private readonly ILogger<BooksController> _logger;

    public BooksController(IBookRepository bookRepository, IMemoryCache cache, ILogger<BooksController> logger)
    {
        _bookRepository = bookRepository;
        _cache = cache;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] AddBookDto bookDto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var book = new Book
            {
                Title = bookDto.Title,
                Author = bookDto.Author,
                ISBN = bookDto.ISBN,
                PublishedDate = bookDto.PublishedDate
            };

            await _bookRepository.AddAsync(book);
            CacheHelper.InvalidateCache(_cache, "BooksCache");
            LoggingHelper.LogOperation(_logger, "Book added", book.Id);

            return Ok(new BaseResponse<int>
            {
                Data = book.Id,
                Success = true,
                Message = "Book added successfully"
            });
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBooks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var cacheKey = CacheHelper.GenerateCacheKey("Books", pageNumber, pageSize);

        if (!_cache.TryGetValue(cacheKey, out PaginatedResponse<List<BookDto>> response))
        {
            var totalBooks = await _bookRepository.GetTotalCountAsync();
            var books = await _bookRepository.GetAllAsync(pageNumber, pageSize);

            var bookDtos = books.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                ISBN = b.ISBN,
                PublishedDate = b.PublishedDate
            }).ToList();

            response = new PaginatedResponse<List<BookDto>>(
                bookDtos,
                totalBooks,
                pageNumber,
                pageSize,
                success: true,
                message: "Books retrieved successfully"
            );

            _cache.Set(cacheKey, response, TimeSpan.FromMinutes(10));
            LoggingHelper.LogOperation(_logger, "Cache miss for books", pageNumber);
        }
        else
        {
            LoggingHelper.LogOperation(_logger, "Cache hit for books", pageNumber);
        }

        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDto bookDto)
    {
        if (id != bookDto.Id)
            return BadRequest(ModelState);

        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return NotFound(new BaseResponse<bool> { Data = false, Success = false, Message = "Book not found" });

        book.Title = bookDto.Title;
        book.Author = bookDto.Author;
        book.ISBN = bookDto.ISBN;
        book.PublishedDate = bookDto.PublishedDate;

        await _bookRepository.UpdateAsync(book);
        CacheHelper.InvalidateCache(_cache, "BooksCache");
        LoggingHelper.LogOperation(_logger, "Book updated", id);

        return Ok(new BaseResponse<bool> { Data = true, Success = true, Message = "Book updated successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
            return NotFound(new BaseResponse<bool> { Data = false, Success = false, Message = "Book not found" });

        await _bookRepository.DeleteAsync(id);
        CacheHelper.InvalidateCache(_cache, "BooksCache");
        LoggingHelper.LogOperation(_logger, "Book deleted", id);

        return Ok(new BaseResponse<bool> { Data = true, Success = true, Message = "Book deleted successfully" });
    }
}
