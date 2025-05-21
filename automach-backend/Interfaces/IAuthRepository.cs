using automach_backend.Dto.Auth;
using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IAuthRepository
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto);
        Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto registerDto);
    }
} 