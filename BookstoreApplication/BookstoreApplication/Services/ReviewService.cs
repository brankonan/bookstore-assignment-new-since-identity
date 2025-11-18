using System.Linq;
using System.Threading.Tasks;
using System;
using BookstoreApplication.Models;
using BookstoreApplication.Repositories;

namespace BookstoreApplication.Services
{
    public class ReviewService
    {
        private readonly IReviewRepository _reviews;
        private readonly IBookRepository _books;
        private readonly IUnitOfWork _uow;

        public ReviewService(IReviewRepository reviews, IBookRepository books, IUnitOfWork uow)
        {
            _reviews = reviews;
            _books = books;
            _uow = uow;
        }

        public async Task<bool> AddReviewAsync(string userId, int bookId, int rating, string? comment)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var book = await _books.GetByIdAsync(bookId, false);
                if (book == null) return false;

                var review = new Review
                {
                    BookId = bookId,
                    UserId = userId,
                    Rating = rating,
                    Comment = comment
                };

                await _reviews.AddAsync(review);
                var all = await _reviews.GetReviewsByBookIdAsync(bookId);
                double newAvg = (all.Select(r => r.Rating).Sum() + rating) / (all.Count + 1.0);
                book.AverageRating = newAvg;

                await _books.UpdateAsync(book);

                await _uow.CommitAsync();
                return true;
            }
            catch
            {
                await _uow.RollbackAsync();
                throw;
            }
        }
    }
}
