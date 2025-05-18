namespace automach_backend.Dto.Account
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // public string Role { get; set; } = string.Empty; // Admin, User
    }
}