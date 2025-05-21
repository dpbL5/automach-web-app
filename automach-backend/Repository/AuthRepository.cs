using Microsoft.EntityFrameworkCore;
using automach_backend.Data;
using automach_backend.Dto.Auth;
using automach_backend.Helpers;
using automach_backend.Interfaces;
using automach_backend.Models;

namespace automach_backend.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public AuthRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginDto)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == loginDto.Username && a.Password == loginDto.Password);

            if (account == null)
            {
                return null;
            }

            var jwtSecretKey = _configuration["Jwt:Key"] ?? "defaultSecretKey12345678901234567890";
            var issuer = _configuration["Jwt:Issuer"] ?? "Automach";
            var audience = _configuration["Jwt:Audience"] ?? "Automach";
            var token = JwtHelper.GenerateJwtToken(account, jwtSecretKey, issuer, audience);

            return new LoginResponseDto
            {
                Token = token,
                AccountId = account.Id,
                Username = account.Username,
                Name = account.Name,
                Role = account.Role
            };
        }

        public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto registerDto)
        {
            // Check if username is already taken
            if (await _context.Accounts.AnyAsync(a => a.Username == registerDto.Username))
            {
                return null;
            }

            // Check if email is already registered
            if (await _context.Accounts.AnyAsync(a => a.Email == registerDto.Email))
            {
                return null;
            }

            // Create new account
            var account = new Account
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                Username = registerDto.Username,
                Password = registerDto.Password,
                CreatedAt = DateTime.Now,
                Role = "user" // Default role for new registrations
            };

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            return new RegisterResponseDto
            {
                Id = account.Id,
                Username = account.Username,
                Email = account.Email,
                Name = account.Name
            };
        }
    }
} 