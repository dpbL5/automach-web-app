using automach_backend.Dto.Game;
using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IGameRepository
    {
        Task<List<Game>> GetAllAsync();
        Task<Game?> GetByIdAsync(int id);
        Task<Game?> CreateAsync(Game gamemModel);
        Task<Game?> UpdateAsync(int id, UpdateGameRequestDto gameDto);
        Task<Game?> DeleteAsync(int id);
    }
}