namespace automach_backend.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public float TotalPrice { get; set; }
    }
}