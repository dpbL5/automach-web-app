using automach_backend.Data;
using automach_backend.Dto.Account;
using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync();
        Task<Account?> GetByIdAsync(int id);
        Task<Account?> CreateAsync(Account account);
        Task<Account?> UpdateAsync(int id, UpdateAccountRequestDto accountDto);
        Task<Account?> DeleteAsync(int id);
        Task<bool> AccountExists(int id);
    }
}