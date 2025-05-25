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

        public async Task<ICollection<ImageUrl>> GetImagesByGameIdAsync(int gameId)
        {
            return await _context.ImageUrls
                .Where(i => i.GameId == gameId)
                .ToListAsync();
        }

        public async Task<ImageUrl?> GetByIdAsync(int id)
        {
            return await _context.ImageUrls.FindAsync(id);
        }

        public async Task<ImageUrl> CreateAsync(ImageUrl imageUrl)
        {
            await _context.ImageUrls.AddAsync(imageUrl);
            await _context.SaveChangesAsync();
            return imageUrl;
        }

        public async Task<ImageUrl?> DeleteAsync(int id)
        {
            var imageUrl = await _context.ImageUrls.FindAsync(id);
            if (imageUrl == null)
            {
                return null;
            }

            _context.ImageUrls.Remove(imageUrl);
            await _context.SaveChangesAsync();
            return imageUrl;
        }
    }
}