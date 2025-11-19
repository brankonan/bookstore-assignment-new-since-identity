using System;
using System.Threading.Tasks;
using BookstoreApplication.DTOs;
using BookstoreApplication.Repositories.Comics;

namespace BookstoreApplication.Services.Comics
{
    public class ComicIssueService : IComicIssueService
    {
        private readonly ComicVineService _comicVineService;
        private readonly IComicIssueRepository _repo;

        public ComicIssueService(
            ComicVineService comicVineService,
            IComicIssueRepository repo)
        {
            _comicVineService = comicVineService;
            _repo = repo;
        }

        public async Task<LocalComicIssueDetails> CreateLocalIssueAsync(SaveIssueDto dto)
        {
            if (dto.ComicVineIssueId == 0)
                throw new ArgumentException("Nedostaje eksterni ComicVine Issue ID.");

            if (await _repo.ExistsByExternalIdAsync(dto.ComicVineIssueId))
                throw new InvalidOperationException("Lokalno izdanje vec postoji za dati ComicVine ID.");

            var external = await _comicVineService.GetSingleIssueAsync(dto.ComicVineIssueId);
            if (external == null)
                throw new InvalidOperationException("Ne postoji izdanje na ComicVine API za zadati ID.");

            var now = DateTime.UtcNow;
            var local = new LocalComicIssueDetails
            {
                ComicVineIssueId = external.Id,
                Name = external.Name,
                Description = external.Description,
                CoverUrl = external.CoverUrl,
                ReleaseDate = external.ReleaseDate,
                IssueNumber = external.IssueNumber,
                PageCount = external.PageCount,
                Price = dto.Price,
                Stock = dto.Stock,
                CreatedAt = now
            };

            var saved = await _repo.InsertAsync(local);
            return saved;
        }
    }
}

