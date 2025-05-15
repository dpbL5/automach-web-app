namespace automach_backend.Models
{
    public class GameTag
    {
        public int TagId { get; set; }
        public Tag Tag { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
    }
}