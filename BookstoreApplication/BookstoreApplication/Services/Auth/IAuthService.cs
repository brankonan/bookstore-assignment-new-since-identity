using System.Security.Claims;
using BookstoreApplication.DTOs;
using BookstoreApplication.Models;

namespace BookstoreApplication.Services.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync(RegistrationDto data);
        Task<string> LoginAsync(LoginDto data);
        Task<ProfileDto> GetProfileAsync(ClaimsPrincipal user);
        Task<string> GenerateJwtForUser(ApplicationUser user);
    }
}
