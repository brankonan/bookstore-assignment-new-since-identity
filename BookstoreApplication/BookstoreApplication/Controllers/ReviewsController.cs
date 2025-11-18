using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using BookstoreApplication.DTOs;
using BookstoreApplication.Services;

namespace BookstoreApplication.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewService _service;
        public ReviewsController(ReviewService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReviewDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest("Rating 1-5");
            var res = await _service.AddReviewAsync(userId, dto.BookId, dto.Rating, dto.Comment);

            return res ? Ok() : NotFound("Book not found");
        }
    }
}
