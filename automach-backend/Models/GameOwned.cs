namespace automach_backend.Models
{
    public class GameOwned
    {
        public int? AccountId { get; set; }
        public Account? Account { get; set; }
        public int? GameId { get; set; }
        public Game? Game { get; set; }
    }
}