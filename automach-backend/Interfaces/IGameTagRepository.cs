using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IGameTagRepository
    {
        Task<List<Tag>> GetTagsByGameIdAsync(int gameId);
        Task<List<Game>> GetGamesByTagIdAsync(int tagId);
        Task<GameTag> AddTagToGameAsync(int gameId, int tagId);
        Task<GameTag?> RemoveTagFromGameAsync(int gameId, int tagId);
    }
} 