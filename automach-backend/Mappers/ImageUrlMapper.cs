using automach_backend.Dto.ImageUrl;
using automach_backend.Models;

namespace automach_backend.Mappers
{
    public static class ImageUrlMapper
    {
        public static ImageUrlResponseDto ToDto(this Models.ImageUrl imageUrl)
        {
            return new ImageUrlResponseDto
            {
                Id = imageUrl.Id,
                Url = imageUrl.Url,
                GameId = imageUrl.GameId
            };
        }

        public static Models.ImageUrl ToModel(this CreateImageUrlRequestDto dto, int gameId)
        {
            return new Models.ImageUrl
            {
                Url = dto.Url,
                GameId = gameId
            };
        }
    }
} 