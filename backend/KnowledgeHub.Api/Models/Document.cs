namespace KnowledgeHub.Api.Models
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // FK
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // Navigation
        public ICollection<DocumentSection> Sections { get; set; } = new List<DocumentSection>();
    }
}
