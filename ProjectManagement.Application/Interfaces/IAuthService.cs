using System.Threading.Tasks;
using ProjectManagement.Application.DTOs;

namespace ProjectManagement.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserDto> RegisterAsync(UserRegisterDto registerDto);
        Task<UserDto> LoginAsync(UserLoginDto loginDto);
    }
}
