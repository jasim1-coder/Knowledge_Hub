using OpenAI.Chat;

namespace KnowledgeHub.Api.Services
{
    internal class ChatCompletionRequest
    {
        public string Model { get; set; }
        public List<ChatMessage> Messages { get; set; }
    }
}