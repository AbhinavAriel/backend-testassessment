using Assessment.Application.DTOs.Auth;
using Assessment.Application.Interfaces;
using Assessment.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Assessment.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var email = (dto.Email ?? "").Trim();

            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
                throw new Exception("Email already registered.");

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = (dto.FullName ?? "").Trim(),
                Email = email,
                UserName = email,
                PhoneNumber = (dto.PhoneNumber ?? "").Trim(),
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var internalPassword = GenerateInternalPassword();

            var result = await _userManager.CreateAsync(user, internalPassword);
            if (!result.Succeeded)
                throw new Exception(string.Join(" | ", result.Errors.Select(e => e.Description)));

            return new RegisterResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? "",
                PhoneNumber = user.PhoneNumber ?? ""
            };
        }

        private static string GenerateInternalPassword()
        {
            return $"Tmp@{Guid.NewGuid():N}A1!";
        }
    }
}