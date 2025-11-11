namespace BookstoreApplication.DTOs
{
    public class ProfileDto
    {
        public string Id { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}
