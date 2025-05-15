namespace automach_backend.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<GameTag> GameTags { get; set; }
    }
}
