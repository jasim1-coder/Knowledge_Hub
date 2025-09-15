namespace KnowledgeHub.Api.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Navigation
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    }
}
