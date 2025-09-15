namespace KnowledgeHub.Api.Models
{
    public class Chat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
