using BookstoreApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BookstoreApplication.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<ICollection<Review>> GetReviewsByBookIdAsync (int bookId);
    }
}
