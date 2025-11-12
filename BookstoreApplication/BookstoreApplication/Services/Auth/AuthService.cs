using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookstoreApplication.DTOs;
using BookstoreApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BookstoreApplication.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;
        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task RegisterAsync(RegistrationDto data)
        {
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
            await _userManager.AddToRoleAsync(user, "Bibliotekar");
        }

        public async Task<string> LoginAsync(LoginDto data)
        {
            var user = await _userManager.FindByNameAsync(data.Username);
            if (user is null)
                throw new ArgumentException("Invalid credentials.");

            var ok = await _userManager.CheckPasswordAsync(user, data.Password);
            if (!ok)
                throw new ArgumentException("Invalid credentials.");

            return await GenerateJwtForUser(user);
        }

        public async Task<ProfileDto> GetProfileAsync(ClaimsPrincipal userPrincipal)
        {
            var username = userPrincipal.FindFirstValue("username");
            if (username is null)
                throw new ArgumentException("Token is invalid.");

            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
                throw new ArgumentException("User does not exist.");

            return new ProfileDto
            {
                Id = user.Id,
                Username = user.UserName!,
                Email = user.Email,
                Name = user.Name,
                Surname = user.Surname
            };
        }

        public async Task<string> GenerateJwtForUser(ApplicationUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        new Claim("username", user.UserName ?? user.Email ?? ""),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
