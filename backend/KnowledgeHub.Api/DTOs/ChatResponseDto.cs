namespace KnowledgeHub.Api.DTOs
{
    public class ChatResponseDto
    {
        public string Answer { get; set; } = string.Empty;
        public List<string> Sources { get; set; } = new List<string>();
    }
}
