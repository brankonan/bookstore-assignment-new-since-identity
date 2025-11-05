using System.ComponentModel.DataAnnotations;

namespace BookstoreApplication.DTOs
{
    public class RegistrationDto
    {
        [Required, EmailAddress] public string Email { get; set; } = default!;
        [Required] public string Username { get; set; } = default!;
        [Required] public string Password { get; set; } = default!;
        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}
