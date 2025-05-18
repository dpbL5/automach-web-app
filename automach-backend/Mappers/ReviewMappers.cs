
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
                GameId = review.GameId,
                AccountId = review.AccountId,
                Content = review.Content,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt
            };
        }

        public static Review ToModel(this CreateReviewRequestDto dto)
        {
            return new Review
            {
                Id = dto.Id,
                GameId = dto.GameId,
                AccountId = dto.AccountId,
                Content = dto.Content,
                Rating = dto.Rating,
                CreatedAt = dto.CreatedAt
            };
        }
    }
}