
using BookstoreApplication.Models;

namespace BookstoreApplication.Repositories
{
    public interface IComicIssueRepository : IRepository<ComicIssue>
    {
        Task<bool> ExistsByComicVineIdAsync(int comicVineIssueId);
        Task<List<ComicIssue>> GetAllAsync(bool asNoTracking = true); // opciono
    }
}

