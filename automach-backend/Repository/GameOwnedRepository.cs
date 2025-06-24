using automach_backend.Data;
using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
    public class GameOwnedRepository : IGameOwnedRepository
    {
        private readonly DataContext _context;

        public GameOwnedRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Game>> GetOwnedGamesByAccountIdAsync(int accountId)
        {
            var games = await _context.GamesOwned
                .Where(go => go.AccountId == accountId)
                .Select(go => go.Game!)
                .ToListAsync();
                
            return games;
        }

        public async Task<GameOwned> AddGameToAccountAsync(int accountId, int gameId)
        {
            var gameOwned = new GameOwned
            {
                AccountId = accountId,
                GameId = gameId
            };

            _context.GamesOwned.Add(gameOwned);
            await _context.SaveChangesAsync();
            return gameOwned;
        }

        public async Task<bool> IsGameOwnedByAccountAsync(int accountId, int gameId)
        {
            return await _context.GamesOwned
                .AnyAsync(go => go.AccountId == accountId && go.GameId == gameId);
        }
    }
} 