
using automach_backend.Dto.Game;
using automach_backend.Interfaces;
using automach_backend.Models;
using automach_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
  public class GameRepository : IGameRepository
  { 
    private readonly DataContext context;
    public GameRepository(DataContext context)
    {
      this.context = context;
    }

    public async Task<Game?> CreateAsync(Game gameModel)
    {
      await context.Games.AddAsync(gameModel);
      await context.SaveChangesAsync();
      return gameModel;
    }

    public Task<Game?> DeleteAsync(int id)
    {
      throw new NotImplementedException();
    }

    public Task<List<Game>> GetAllAsync()
    {
      throw new NotImplementedException();
    }

    public Task<Game?> GetByIdAsync(int id)
    {
      throw new NotImplementedException();
    }

    public Task<Game?> UpdateAsync(int id, UpdateGameRequestDto gameDto)
    {
      throw new NotImplementedException();
    }
  }
}