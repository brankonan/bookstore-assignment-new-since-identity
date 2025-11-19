namespace BookstoreApplication.Models
{
    public class ComicIssue
    {
        public int Id { get; set; }
        public int ComicVineIssueId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CoverUrl { get; set; }
        public string ReleaseDate { get; set; }
        public string IssueNumber { get; set; }
        public int PageCount { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
