using automach_backend.Data;
using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
    public class ImageUrlRepository : IImageUrlRepository
    {
        private readonly DataContext _context;

        public ImageUrlRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetImageUrlsByGameId(int gameId)
        {
            return await _context.Set<ImageUrl>()
                .Where(img => img.GameId == gameId)
                .Select(img => img.Url)
                .ToListAsync();
        }

        public async Task<string> CreateImageUrl(string url, int gameId)
        {
            var imageUrl = new ImageUrl
            {
                Url = url,
                GameId = gameId
            };

            _context.Set<ImageUrl>().Add(imageUrl);
            await _context.SaveChangesAsync();
            return url;
        }

        public async Task<bool> DeleteImageUrl(int id)
        {
            var imageUrl = await _context.Set<ImageUrl>().FindAsync(id);
            if (imageUrl == null)
            {
                return false;
            }

            _context.Set<ImageUrl>().Remove(imageUrl);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateImageUrl(int id, string url)
        {
            var imageUrl = await _context.Set<ImageUrl>().FindAsync(id);
            if (imageUrl == null)
            {
                return false;
            }

            imageUrl.Url = url;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteImageUrlsByGameId(int gameId)
        {
            var imageUrls = await _context.Set<ImageUrl>()
                .Where(img => img.GameId == gameId)
                .ToListAsync();

            if (!imageUrls.Any())
            {
                return false;
            }

            _context.Set<ImageUrl>().RemoveRange(imageUrls);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}