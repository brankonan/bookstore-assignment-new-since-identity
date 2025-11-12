using BookstoreApplication.Models;
using Microsoft.AspNetCore.Mvc;
using BookstoreApplication.Repositories;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookstoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _books;
        private readonly IAuthorRepository _authors;
        private readonly IPublisherRepository _publisher;

        public BooksController(IBookRepository books, IAuthorRepository authors, IPublisherRepository publishers)
        {
            _books = books;
            _authors = authors;
            _publisher = publishers;
        }

        // GET: api/books
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _books.GetAllWithIncludesAsync());

        // GET api/books/5
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOne(int id)
        {
            var book = await _books.GetOneWithIncludesAsync(id);
            return book is null ? NotFound() : Ok(book);
        }
        [Authorize(Policy = "CreateBooks")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Book dto)
        {
            if (!await _authors.ExistsAsync(dto.AuthorId))
                return BadRequest($"Nepostojeci autor id: {dto.AuthorId}");
            if (!await _publisher.ExistsAsync(dto.PublisherId))
                return BadRequest($"Nepostojeci izdavac id: {dto.PublisherId}");

            var created = await _books.AddAsync(dto);
            await _books.SaveChangesAsync();

            var withRefs = await _books.GetOneWithIncludesAsync(created.Id);

            return CreatedAtAction(nameof(GetOne), new { id = created.Id }, withRefs);
        }

        [Authorize(Policy = "EditBooks")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] Book dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");
            if (!await _books.ExistsAsync(id)) return NotFound();
            if (!await _authors.ExistsAsync(dto.AuthorId)) return BadRequest($"Nepostojeci autor id: {dto.AuthorId}");
            if (!await _publisher.ExistsAsync(dto.PublisherId)) return BadRequest($"Nepostojeci izdavac: {dto.PublisherId}");

            await _books.UpdateAsync(dto);
            await _books.SaveChangesAsync();

            var withRefs = await _books.GetOneWithIncludesAsync(id);
            return Ok(withRefs);
        }

        [Authorize(Policy = "EditBooks")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _books.DeleteAsync(id);
            await _books.SaveChangesAsync();
            return NoContent();
        }
    }
}
