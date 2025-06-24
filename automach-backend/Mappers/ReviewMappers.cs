using automach_backend.Dto.Review;
using automach_backend.Models;

namespace automach_backend.Mappers
{
    public static class ReviewMappers
    {
        public static ReviewDto ToDto(this Review review)
        {
            return new ReviewDto
            {
                Id = review.Id,
                GameId = review.GameId,
                AccountId = review.AccountId,
                Content = review.Content,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt
            };
        }

        public static Review ToModel(this CreateReviewRequestDto dto, int accountId, int gameId)
        {
            return new Review
            {
                GameId = gameId,
                AccountId = accountId,
                Content = dto.Content,
                Rating = dto.Rating,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}