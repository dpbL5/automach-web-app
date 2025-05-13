namespace automach_backend.Models;

public class Review
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int AccountId { get; set; }
    public string ReviewText { get; set; } = string.Empty;
    public int Rating { get; set; } // from 1 to 100
    public DateTime CreatedAt { get; set; }
}