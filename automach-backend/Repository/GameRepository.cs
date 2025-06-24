using automach_backend.Dto.Game;
using automach_backend.Interfaces;
using automach_backend.Models;
using automach_backend.Data;
using Microsoft.EntityFrameworkCore;
using automach_backend.Helpers;

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

		public Task<bool> GameExists(int gameId)
		{
			return context.Games.AnyAsync(g => g.Id == gameId);
		}

		public async Task<(List<Game> Games, int TotalCount)> GetAllAsync(QueryObject query)
		{
			var games = context.Games
				.Include(r => r.Reviews)
				.Include(g => g.GameTags)
					.ThenInclude(gt => gt.Tag)
				.Include(g => g.ImageUrls)
				.AsQueryable();

			// Apply filtering
			if (!string.IsNullOrEmpty(query.Title))
			{
				games = games.Where(g => g.Title.Contains(query.Title));
			}
			
			// Filter by single tag (for backward compatibility)
			if (!string.IsNullOrEmpty(query.GameTag))
			{
				games = games.Where(g => g.GameTags!.Any(gt => gt.Tag!.Title == query.GameTag));
			}
			
			// Filter by multiple tags
			if (query.GameTags != null && query.GameTags.Any())
			{
				foreach (var tag in query.GameTags)
				{
					games = games.Where(g => g.GameTags!.Any(gt => gt.Tag!.Title == tag));
				}
			}
			
			if (query.IsFeatured.HasValue)
			{
				games = games.Where(g => g.IsFeatured == query.IsFeatured.Value);
			}
			
			// Filter by max price
			if (query.MaxPrice.HasValue)
			{
				games = games.Where(g => g.Price <= query.MaxPrice.Value);
			}
			
			// Get total count before pagination
			var totalCount = await games.CountAsync();
			
			// Apply pagination
			var pagedGames = await games
				.Skip((query.PageNumber - 1) * query.PageSize)
				.Take(query.PageSize)
				.ToListAsync();
				
			return (pagedGames, totalCount);
		}

		public async Task<Game?> GetByIdAsync(int id)
		{
			return await context.Games
				.Include(g => g.Reviews)
				.Include(g => g.GameTags)
					.ThenInclude(gt => gt.Tag)
				.Include(g => g.ImageUrls)
				.FirstOrDefaultAsync(a => a.Id == id);
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