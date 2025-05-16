namespace automach_backend.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; }
        public float TotalPrice { get; set; }
        
        public Account Account { get; set; }
        public ICollection<TransactionItem> TransactionItems { get; set; } // Phuc vu xem chi tiet giao dich
    }
}