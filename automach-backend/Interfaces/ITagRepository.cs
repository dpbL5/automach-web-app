using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface ITagRepository
    {
        Task<List<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag> CreateAsync(Tag tag);
        Task<Tag?> UpdateAsync(int id, Tag tag);
        Task<Tag?> DeleteAsync(int id);
    }
} 