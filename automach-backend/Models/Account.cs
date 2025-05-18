namespace automach_backend.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Role { get; set; } = string.Empty; // Admin, User
        public ICollection<Review>? Reviews { get; set; } // Phuc vu xem danh sach danh gia
        public ICollection<Transaction>? Transactions { get; set; } // Phuc vu xem lich su giao dich
        public ICollection<GameOwned>? GamesOwned { get; set; } // Phuc vu xem danh sach game da mua
    }

}