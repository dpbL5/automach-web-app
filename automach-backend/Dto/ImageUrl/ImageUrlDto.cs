using automach_backend.Models;
using System.ComponentModel.DataAnnotations;

namespace automach_backend.Dto.ImageUrl
{
    public class ImageUrlDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int GameId { get; set; }
    }

    public class CreateImageUrlRequestDto
    {
        [Required]
        public string Url { get; set; } = string.Empty;
    }

    public class ImageUrlResponseDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int GameId { get; set; }
    }
}