using System;
using System.Threading.Tasks;
using BookstoreApplication.DTOs;
using BookstoreApplication.Models;
using BookstoreApplication.Repositories;

namespace BookstoreApplication.Services
{
    public class ComicIssueService : IComicIssueService
    {
        private readonly ComicVineService _comicVineService;
        private readonly IComicIssueRepository _comicIssueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ComicIssueService(
            ComicVineService comicVineService,
            IComicIssueRepository comicIssueRepository,
            IUnitOfWork unitOfWork)
        {
            _comicVineService = comicVineService;
            _comicIssueRepository = comicIssueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<LocalComicIssueDetails> CreateLocalIssueAsync(SaveIssueDto dto)
        {
            if (dto.ComicVineIssueId == 0)
                throw new ArgumentException("ComicVineIssueId is required", nameof(dto.ComicVineIssueId));

            if (await _comicIssueRepository.ExistsByComicVineIdAsync(dto.ComicVineIssueId))
                throw new InvalidOperationException("Lokalno izdanje vec postoji za dati ComicVine ID.");

            var external = await _comicVineService.GetSingleIssueAsync(dto.ComicVineIssueId);

            if (external == null)
                throw new InvalidOperationException("Ne postoji izdanje na ComicVine API za zadati ID.");

            var entity = new ComicIssue
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
                CreatedAt = DateTime.UtcNow
            };

            await _comicIssueRepository.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            return new LocalComicIssueDetails
            {
                Id = entity.Id,
                ComicVineIssueId = entity.ComicVineIssueId,
                Name = entity.Name,
                Description = entity.Description,
                CoverUrl = entity.CoverUrl,
                ReleaseDate = entity.ReleaseDate,
                IssueNumber = entity.IssueNumber,
                PageCount = entity.PageCount,
                Price = entity.Price,
                Stock = entity.Stock,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
