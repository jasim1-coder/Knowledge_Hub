namespace KnowledgeHub.Api.Models
{
    public class DocumentSection
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty;

        public int Order { get; set; }

        public int SourcePage { get; set; } // optional: page number for PDFs



        // FK
        public Guid DocumentId { get; set; }
        public Document Document { get; set; } = null!;

        // Embedding vector stored as JSON or string
        public string? EmbeddingJson { get; set; }
    }
}
