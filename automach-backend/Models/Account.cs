namespace automach_backend.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } 
        public string PhoneNumber { get; set; } = string.Empty;
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } // Phuc vu phan quyen
        public ICollection<Review> Reviews { get; set; } // Phuc vu xem danh sach danh gia
        public ICollection<Transaction> Transactions { get; set; } // Phuc vu xem lich su giao dich
        public ICollection<GameOwned> GamesOwned { get; set; } // Phuc vu xem danh sach game da mua
    }

}