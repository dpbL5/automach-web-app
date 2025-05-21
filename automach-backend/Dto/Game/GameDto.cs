
namespace automach_backend.Dto.Game
{
    public class GameDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public float Price { get; set; }
        public string? GameInfo { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Developer { get; set; } = string.Empty;
        
    }
}