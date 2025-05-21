using automach_backend.Data;
using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
    public class GameTagRepository : IGameTagRepository
    {
        private readonly DataContext _context;

        public GameTagRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetTagsByGameIdAsync(int gameId)
        {
            var tags = await _context.GameTags
                .Where(gt => gt.GameId == gameId)
                .Select(gt => gt.Tag!)
                .ToListAsync();
                
            return tags;
        }

        public async Task<GameTag> AddTagToGameAsync(int gameId, int tagId)
        {
            var gameTag = new GameTag
            {
                GameId = gameId,
                TagId = tagId
            };

            _context.GameTags.Add(gameTag);
            await _context.SaveChangesAsync();
            return gameTag;
        }

        public async Task<GameTag?> RemoveTagFromGameAsync(int gameId, int tagId)
        {
            var gameTag = await _context.GameTags
                .FirstOrDefaultAsync(gt => gt.GameId == gameId && gt.TagId == tagId);

            if (gameTag == null)
            {
                return null;
            }

            _context.GameTags.Remove(gameTag);
            await _context.SaveChangesAsync();
            return gameTag;
        }
    }
} 