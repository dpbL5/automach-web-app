using automach_backend.Dto.Account;
using automach_backend.Models;

namespace automach_backend.Mappers
{
    public static class AccountMappers
    {
        public static AccountDto ToDto(this Account account)
        {
            return new AccountDto
            {
                Id = account.Id,
                Name = account.Name,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                Username = account.Username,
                Password = account.Password,
                CreatedAt = account.CreatedAt,
                Role = account.Role
            };
        }

        public static Account ToModel(this CreateAccountRequestDto accountDto)
        {
            return new Account
            {
                Name = accountDto.Name,
                Email = accountDto.Email,
                PhoneNumber = accountDto.PhoneNumber,
                Username = accountDto.Username,
                Password = accountDto.Password,
                CreatedAt = accountDto.CreatedAt,
                Role = accountDto.Role
            };
        }
    }
}