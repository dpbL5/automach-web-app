using automach_backend.Models;

namespace automach_backend.Dto.ImageUrl
{
    public class ImageUrlDto
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int GameId { get; set; }
    }
}