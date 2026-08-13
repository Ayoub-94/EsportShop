using EsportShop.Api.DTOs;

namespace EsportShop.Api.Services
{
    public interface IAuthService
    {
        Task<UserResponseDto> RegisterAsync(UserRegisterDto request);
        Task<string> LoginAsync(UserLoginDto request);
    }
}
