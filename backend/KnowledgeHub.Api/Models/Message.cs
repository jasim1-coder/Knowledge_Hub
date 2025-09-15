namespace KnowledgeHub.Api.Models
{
    public class Message
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Role { get; set; } = "user"; // user or assistant
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // FK
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = null!;
    }
}
