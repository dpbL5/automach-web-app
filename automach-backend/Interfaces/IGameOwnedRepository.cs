using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IGameOwnedRepository
    {
        Task<List<Game>> GetOwnedGamesByAccountIdAsync(int accountId);
        Task<GameOwned> AddGameToAccountAsync(int accountId, int gameId);
        Task<bool> IsGameOwnedByAccountAsync(int accountId, int gameId);
    }
} 