using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookstoreApplication.Models;
using BookstoreApplication.DTOs;

namespace BookstoreApplication.Repositories.Comics;

public class ComicIssueRepository : IComicIssueRepository
{
    private readonly AppDbContext _db;

    public ComicIssueRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> ExistsByExternalIdAsync(int comicVineIssueId)
    {
        return _db.ComicIssues.AnyAsync(ci => ci.ComicVineIssueId == comicVineIssueId);
    }

    public async Task<LocalComicIssueDetails> InsertAsync(LocalComicIssueDetails issue)
    {
        var entity = new ComicIssue
        {
            ComicVineIssueId = issue.ComicVineIssueId,
            Name = issue.Name,
            Description = issue.Description,
            CoverUrl = issue.CoverUrl,
            ReleaseDate = issue.ReleaseDate,
            IssueNumber = issue.IssueNumber,
            PageCount = issue.PageCount,
            Price = issue.Price,
            Stock = issue.Stock,
            CreatedAt = DateTime.UtcNow
        };

        _db.ComicIssues.Add(entity);
        await _db.SaveChangesAsync();

        issue.Id = entity.Id;
        return issue;
    }
}

