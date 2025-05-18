using automach_backend.Data;
using automach_backend.Models;
using automach_backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _context;
        public TransactionRepository(DataContext context)
        {
            this._context = context;
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(a => a.Id == id);
            if (transaction == null)
            {
                return null;
            }
            return transaction;
        }

        public async Task<Transaction?> CreateAsync(Transaction TransactionmModel)
        {
            await _context.Transactions.AddAsync(TransactionmModel);
            await _context.SaveChangesAsync();
            return TransactionmModel;
        }
        // public async Task<bool> TransactionExists(int id)
        // {
        //     return await _context.Transactions.AnyAsync(a => a.Id == id);
        // }
  }
}