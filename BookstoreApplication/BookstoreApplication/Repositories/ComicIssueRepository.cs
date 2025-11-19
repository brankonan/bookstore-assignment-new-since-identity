using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookstoreApplication.Models;

namespace BookstoreApplication.Repositories
{
    public class ComicIssueRepository : Repository<ComicIssue>, IComicIssueRepository
    {
        public ComicIssueRepository(AppDbContext db) : base(db)
        {
        }

        public Task<bool> ExistsByComicVineIdAsync(int comicVineIssueId)
        {
            return _set.AnyAsync(ci => ci.ComicVineIssueId == comicVineIssueId);
        }

        public override Task<List<ComicIssue>> GetAllAsync(bool asNoTracking = true)
        {
            return base.GetAllAsync(asNoTracking);
        }
    }
}
