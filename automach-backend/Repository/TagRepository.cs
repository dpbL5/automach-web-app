using automach_backend.Data;
using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _context;

        public TagRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Tag> CreateAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag?> UpdateAsync(int id, Tag updatedTag)
        {
            var existingTag = await _context.Tags.FindAsync(id);
            if (existingTag == null)
            {
                return null;
            }

            existingTag.Title = updatedTag.Title;

            await _context.SaveChangesAsync();
            return existingTag;
        }

        public async Task<Tag?> DeleteAsync(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return null;
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return tag;
        }
    }
} 