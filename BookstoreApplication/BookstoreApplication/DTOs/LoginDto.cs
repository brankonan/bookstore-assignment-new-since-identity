using System.ComponentModel.DataAnnotations;

namespace BookstoreApplication.DTOs
{
    public class LoginDto
    {
        [Required] public string Username { get; set; } = default!;
        [Required] public string Password { get; set; } = default!;
    }
}
