namespace automach_backend.Dto.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
} 