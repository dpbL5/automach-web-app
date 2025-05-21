namespace automach_backend.Models
{
    public class TransactionItem
    {
        public int GameId { get; set; }
        public Game? Game { get; set; }
        public int TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }
}