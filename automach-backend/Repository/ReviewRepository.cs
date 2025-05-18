using automach_backend.Data;
using automach_backend.Dto.Review;
using automach_backend.Interfaces;
using automach_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace automach_backend.Repository
{
    public class ReviewRepository : IReviewRepository
    {   
        private readonly DataContext context;
        public ReviewRepository(DataContext context)
        {
            this.context = context;
        }
        public async Task<Review?> CreateAsync(Review reviewModel, int accountId, int gameId)
        {
            await context.Reviews.AddAsync(reviewModel);
            await context.SaveChangesAsync();
            return reviewModel;
        }

        public async Task<Review?> DeleteAsync(int id)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(a => a.Id == id);
            if (review == null)
            {
                return null;
            }
            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            return review;
        }

        public async Task<List<Review>> GetAllAsync()
        {
            return await context.Reviews.ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            var review = await context.Reviews.FirstOrDefaultAsync(a => a.Id == id);
            if (review == null)
            {
                return null;
            }
            return review;
        }

        public async Task<Review?> UpdateAsync(int id, UpdateReviewRequestDto ReviewDto)
        {
            var existingReview = await context.Reviews.FirstOrDefaultAsync(a => a.Id == id);
            if (existingReview == null)
            {
                return null;
            }

            existingReview.Content = ReviewDto.Content;
            existingReview.Rating = ReviewDto.Rating;
            await context.SaveChangesAsync();
            return existingReview;
        }
    }
}