using Assessment.Application.DTOs.Auth;
using Assessment.Application.Interfaces;
using Assessment.Infrastructure.Identity;
using Assessment.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;

namespace Assessment.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtTokenGenerator jwtTokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var email = (dto.Email ?? string.Empty).Trim();
            var password = dto.Password ?? string.Empty;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
                throw new UnauthorizedAccessException("Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
                throw new UnauthorizedAccessException("You are not allowed to access the admin portal.");

            var jwt = _jwtTokenGenerator.Generate(user, roles);

            return new LoginResponseDto
            {
                Token = jwt.Token,
                ExpiresAtUtc = jwt.ExpiresAtUtc,
                User = new AdminUserDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                }
            };
        }

        private static string GenerateInternalPassword()
        {
            return $"Tmp@{Guid.NewGuid():N}A1!";
        }
    }
}