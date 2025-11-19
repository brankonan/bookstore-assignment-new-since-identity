using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookstoreApplication.Services;
using BookstoreApplication.DTOs;

namespace BookstoreApplication.Controllers
{
    [ApiController]
    [Route("api/volumes")]
    [Authorize(Roles = "Urednik")]
    public class ComicsController : ControllerBase
    {
        private readonly ComicVineService _comicVineService;
        private readonly IComicIssueService _comicIssueService;

        public ComicsController(
            ComicVineService comicVineService,
            IComicIssueService comicIssueService)
        {
            _comicVineService = comicVineService;
            _comicIssueService = comicIssueService;
        }

        // GET api/volumes/search?search=spider  (ili ?filter=, zavisi sta ces na frontu)
        [HttpGet("search")]
        public async Task<IActionResult> GetVolumes([FromQuery] string search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return BadRequest("Parametar 'search' je obavezan.");

            var volumes = await _comicVineService.SearchVolumesAsync(search);
            return Ok(volumes);
        }

        // GET api/volumes/issues?volumeId=12345
        [HttpGet("issues")]
        public async Task<IActionResult> GetIssues([FromQuery] int volumeId)
        {
            if (volumeId == 0)
                return BadRequest("Parametar 'volumeId' je obavezan.");

            var issues = await _comicVineService.GetIssuesForVolumeAsync(volumeId);
            return Ok(issues);
        }

        // POST api/volumes/local-issues
        [HttpPost("local-issues")]
        public async Task<IActionResult> CreateLocalIssue([FromBody] SaveIssueDto dto)
        {
            try
            {
                var created = await _comicIssueService.CreateLocalIssueAsync(dto);
                return Ok(created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("vec postoji"))
                    return Conflict(ex.Message);

                if (ex.Message.Contains("Ne postoji izdanje"))
                    return NotFound(ex.Message);

                return BadRequest(ex.Message);
            }
        }
    }
}
