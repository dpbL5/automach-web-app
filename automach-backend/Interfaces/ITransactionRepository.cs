using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(int id);
        Task<Transaction?> CreateAsync(Transaction TransactionmModel);
        // Task<bool> TransactionExists(int id);
    }
}