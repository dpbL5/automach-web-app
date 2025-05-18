

using automach_backend.Dto.Review;
using automach_backend.Models;

namespace automach_backend.Interfaces
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllAsync();
        Task<Review?> GetByIdAsync(int id);
        Task<Review?> CreateAsync(Review ReviewModel, int accountId, int gameId);
        Task<Review?> UpdateAsync(int id, UpdateReviewRequestDto ReviewDto);
        Task<Review?> DeleteAsync(int id);
    }
}