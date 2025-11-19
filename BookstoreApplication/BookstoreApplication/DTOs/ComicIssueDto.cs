namespace BookstoreApplication.DTOs
{
    public class ComicIssueDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CoverUrl { get; set; }
        public string IssueNumber { get; set; }
        public string Description { get; set; }
        public string ReleaseDate { get; set; }
        public int PageCount { get; set; }
    }
}
