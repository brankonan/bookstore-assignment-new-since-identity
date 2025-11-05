using BookstoreApplication.DTOs;
using BookstoreApplication.Models;
using Microsoft.AspNetCore.Identity;

namespace BookstoreApplication.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task RegisterAsync(RegistrationDto data)
        {
            // provere za email/username
            if (await _userManager.FindByEmailAsync(data.Email) is not null)
                throw new ArgumentException("Email already in use.");

            if (await _userManager.FindByNameAsync(data.Username) is not null)
                throw new ArgumentException("Username already in use.");

            var user = new ApplicationUser
            {
                UserName = data.Username,
                Email = data.Email,
                Name = data.Name,
                Surname = data.Surname,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, data.Password);
            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new ArgumentException(msg);
            }
        }

        public async Task LoginAsync(LoginDto data)
        {
            var user = await _userManager.FindByNameAsync(data.Username);
            if (user is null)
                throw new ArgumentException("Invalid credentials.");

            var ok = await _userManager.CheckPasswordAsync(user, data.Password);
            if (!ok)
                throw new ArgumentException("Invalid credentials.");
        }
    }
}
