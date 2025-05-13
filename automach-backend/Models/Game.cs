namespace automach_backend.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public float Price { get; set; }
        public string GameInfo { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public string Developer { get; set; } = string.Empty;
        public bool IsFeatured { get; set; }
    }

}
