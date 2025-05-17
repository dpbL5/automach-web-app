namespace automach_backend.Dto
{
    public class Account
    {
        public int Id { get; set; }
        // public string Name { get; set; }
        public string Email { get; set; } 
        public string PhoneNumber { get; set; } = string.Empty;
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Role { get; set; } // Phuc vu phan quyen
    }
}