using BookstoreApplication.DTOs;

namespace BookstoreApplication.Services.Auth
{
    public interface IAuthService
    {
        Task RegisterAsync(RegistrationDto data);
        Task LoginAsync(LoginDto data);
    }
}
