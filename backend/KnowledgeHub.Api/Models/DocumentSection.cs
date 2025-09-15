namespace KnowledgeHub.Api.Models
{
    public class DocumentSection
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;

        public int Order { get; set; }


        // FK
        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = null!;
    }
}
