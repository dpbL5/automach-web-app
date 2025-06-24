using automach_backend.Dto.Game;
using automach_backend.Helpers;
using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IGameRepository
    {
        Task<(List<Game> Games, int TotalCount)> GetAllAsync(QueryObject queryObject);
        Task<Game?> GetByIdAsync(int id);
        Task<Game?> CreateAsync(Game gamemModel);
        Task<Game?> UpdateAsync(int id, UpdateGameRequestDto gameDto);
        Task<Game?> DeleteAsync(int id);
        public Task<bool> GameExists(int gameId);
    }
}