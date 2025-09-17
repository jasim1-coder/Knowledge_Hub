namespace KnowledgeHub.Api.DTOs
{
    public class ChatRequestDto
    {
        
            public Guid UserId { get; set; }
            public string Question { get; set; } = string.Empty;
            public List<Guid>? DocumentIds { get; set; } = null;
        }

    }

