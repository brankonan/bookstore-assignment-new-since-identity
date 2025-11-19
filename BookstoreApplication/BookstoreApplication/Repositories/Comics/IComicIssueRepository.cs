using BookstoreApplication.DTOs;

namespace BookstoreApplication.Repositories.Comics
{
    public interface IComicIssueRepository
    {
        Task<bool> ExistsByExternalIdAsync(int comicVineIssueId);

        Task<LocalComicIssueDetails> InsertAsync(LocalComicIssueDetails issue);

        // Task<List<LocalComicIssueDetails>> GetAllAsync();
    }
}