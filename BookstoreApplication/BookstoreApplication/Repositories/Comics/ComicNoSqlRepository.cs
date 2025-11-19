using BookstoreApplication.DTOs;
using BookstoreApplication.Models.Mongo;
using MongoDB.Driver;

namespace BookstoreApplication.Repositories.Comics
{
    public class ComicNoSqlRepository : IComicIssueRepository
    {
        private readonly IMongoCollection<ComicIssueDocument> _collection;

        public ComicNoSqlRepository(IMongoDatabase db)
        {
            _collection = db.GetCollection<ComicIssueDocument>("ComicIssues");
        }

        public async Task<bool> ExistsByExternalIdAsync(int comicVineIssueId)
        {
            var filter = Builders<ComicIssueDocument>.Filter
                .Eq(x => x.ComicVineIssueId, comicVineIssueId);

            var count = await _collection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<LocalComicIssueDetails> InsertAsync(LocalComicIssueDetails issue)
        {
            var doc = new ComicIssueDocument
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
                CreatedAt = issue.CreatedAt
            };

            await _collection.InsertOneAsync(doc);

            issue.Id = 0;
            return issue;
        }
    }
}

