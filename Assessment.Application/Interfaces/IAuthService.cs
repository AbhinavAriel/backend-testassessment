using Assessment.Application.DTOs.Auth;

namespace Assessment.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    }
}