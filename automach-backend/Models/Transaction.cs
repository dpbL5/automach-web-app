namespace automach_backend.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string PaymentMethod { get; set; } = string.Empty; // e.g. "Credit Card", "PayPal", etc.
        public float TotalPrice { get; set; }
        
        public Account Account { get; set; } = new Account(); 
        public ICollection<TransactionItem>? TransactionItems { get; set; }
    }
}