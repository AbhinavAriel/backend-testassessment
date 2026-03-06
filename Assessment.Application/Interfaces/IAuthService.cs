using Assessment.Application.DTOs.Auth;

namespace Assessment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto);
    }
}