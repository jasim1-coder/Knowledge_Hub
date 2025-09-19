namespace KnowledgeHub.Api.DTOs
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<MessageDto> Messages { get; set; } = new();
    }
}
