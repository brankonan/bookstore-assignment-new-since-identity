using BookstoreApplication.DTOs;

namespace BookstoreApplication.Services
{
    public interface IComicIssueService
    {
        Task<LocalComicIssueDetails> CreateLocalIssueAsync(SaveIssueDto dto);
    }
}
