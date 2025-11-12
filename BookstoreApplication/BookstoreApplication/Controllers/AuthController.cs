using BookstoreApplication.DTOs;
using BookstoreApplication.Models;
using BookstoreApplication.Services.Auth;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookstoreApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public AuthController(
                UserManager<ApplicationUser> userManager,
                IAuthService authService,
                IConfiguration config)
        {
            _userManager = userManager;
            _authService = authService;
            _config = config;
        }

        public class GoogleLoginDto
        {
            public string Credential { get; set; } // Google ID token sa fronta
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await _authService.RegisterAsync(dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var token = await _authService.LoginAsync(dto);
                return Ok(new { token });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [Authorize(Policy = "ReadProfile")]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var profile = await _authService.GetProfileAsync(User);
            return Ok(profile);
        }

        [AllowAnonymous]
        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto?.Credential))
                return BadRequest("Missing credential.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(
                    dto.Credential,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _config["GoogleAuth:ClientId"] }
                    });
            }
            catch
            {
                return Unauthorized("Invalid Google token.");
            }

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    Name = payload.GivenName ?? "",
                    Surname = payload.FamilyName ?? "",
                    EmailConfirmed = true
                };
                var res = await _userManager.CreateAsync(user);
                if (!res.Succeeded)
                    return BadRequest(string.Join("; ", res.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(user, "Bibliotekar");
            }

            var jwt = await _authService.GenerateJwtForUser(user);
            return Ok(new { token = jwt });
        }
    }
}
