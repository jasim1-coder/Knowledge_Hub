using KnowledgeHub.Api.Models;

public class Document
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Optional enhancements:
    public long FileSize { get; set; } // File size in bytes
    public string ContentType { get; set; } = string.Empty; // MIME type
    public string? Description { get; set; } // User-provided description
    public DateTime? LastProcessedAt { get; set; } // When embeddings were last generated
    public bool IsProcessed { get; set; } = false; // Whether embeddings are complete
    public DocumentStatus Status { get; set; } = DocumentStatus.Uploaded; // Processing status

    // FK
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation
    public ICollection<DocumentSection> Sections { get; set; } = new List<DocumentSection>();
}

public enum DocumentStatus
{
    Uploaded,
    Processing,
    Processed,
    Failed
}