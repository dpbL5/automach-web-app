using automach_backend.Data;
using automach_backend.Dto.Account;
using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext context;
    
        public AccountRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task<Account?> CreateAsync(Account account)
        {
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> DeleteAsync(int id)
        {
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account == null)
            {
                return null;
            }
            context.Accounts.Remove(account);
            await context.SaveChangesAsync();
            return account;
        }

        public async Task<List<Account>> GetAllAsync()
        {
            return await context.Accounts.ToListAsync();
        }

        public async Task<Account?> GetByIdAsync(int id)
        {
            var account = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account == null)
            {
                return null;
            }
            return account;
        }

        public async Task<Account?> UpdateAsync(int id, UpdateAccountRequestDto accountDto)
        {
            var existingAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (existingAccount == null)
            {
                return null;
            }

            existingAccount.Name = accountDto.Name;
            existingAccount.Email = accountDto.Email;
            existingAccount.PhoneNumber = accountDto.PhoneNumber;
            existingAccount.Username = accountDto.Username;
            existingAccount.Password = accountDto.Password;

            await context.SaveChangesAsync();

            return existingAccount;
        }
  }
}