using System.Threading.Tasks;
using ProjectManagement.Application.DTOs;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Domain.Exceptions;

namespace ProjectManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> RegisterAsync(UserRegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new DomainException("User with this email already exists.");
            }

            var user = new User
            {
                Email = registerDto.Email,
                Name = registerDto.Name,
                PasswordHash = _passwordHasher.HashPassword(registerDto.Password)
            };

            await _userRepository.AddAsync(user);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }

        public async Task<UserDto> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new DomainException("Invalid email or password.");
            }

            if (!_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new DomainException("Invalid email or password.");
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            };
        }
    }
}
