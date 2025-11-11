using System.Security.Claims;
using BookstoreApplication.DTOs;

namespace BookstoreApplication.Services.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync(RegistrationDto data);
        Task<string> LoginAsync(LoginDto data);
        Task<ProfileDto> GetProfileAsync(ClaimsPrincipal user);
    }
}
