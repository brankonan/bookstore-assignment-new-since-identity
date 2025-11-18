using BookstoreApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookstoreApplication.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext db) : base(db) { }

        public async Task<ICollection<Review>> GetReviewsByBookIdAsync(int bookId)
            => await _set.Where(r => r.BookId == bookId).ToListAsync();
    }
}
