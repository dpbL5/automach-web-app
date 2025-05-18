
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

		public async Task<Game?> DeleteAsync(int id)
		{
			var game = await context.Games.FirstOrDefaultAsync(a => a.Id == id);
			if (game == null)
			{
				return null;
			}
			context.Games.Remove(game);
			await context.SaveChangesAsync();
			return game;
		}

		public async Task<List<Game>> GetAllAsync()
		{
			return await context.Games.ToListAsync();
		}

		public async Task<Game?> GetByIdAsync(int id)
		{
			return await context.Games.FirstOrDefaultAsync(a => a.Id == id);
		}

		public async Task<Game?> UpdateAsync(int id, UpdateGameRequestDto gameDto)
		{
			var game = await context.Games.FirstOrDefaultAsync(a => a.Id == id);

			if (game == null)
			{
				return null;
			}

			game.Price = gameDto.Price;
			game.GameInfo = gameDto.GameInfo;
			game.ReleaseDate = gameDto.ReleaseDate;
			game.Developer = gameDto.Developer;
			await context.SaveChangesAsync();
			return game;
		}
  	}
}