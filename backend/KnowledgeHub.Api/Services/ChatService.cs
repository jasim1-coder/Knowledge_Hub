using KnowledgeHub.Api.Data;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;

namespace KnowledgeHub.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly OpenAIClient _openAiClient;

        public ChatService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _openAiClient = new OpenAIClient(config["OpenAI:ApiKey"]);
        }

        public async Task<string> AskQuestionAsync(Guid userId, string question, List<Guid>? documentIds = null)
        {
            // Select documents
            var docsQuery = _context.Documents
                .Where(d => d.UserId == userId)
                .Include(d => d.Sections);

            if (documentIds != null && documentIds.Any())
                docsQuery = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Models.Document, ICollection<Models.DocumentSection>>)docsQuery.Where(d => documentIds.Contains(d.Id));

            var docs = await docsQuery.ToListAsync();

            // Get context text (basic)
            var contextText = string.Join("\n", docs.SelectMany(d => d.Sections).Take(3).Select(s => s.Content));

            var systemPrompt = "You are a helpful assistant. Only answer based on the provided context.";
            var userPrompt = $"Context:\n{contextText}\n\nQuestion: {question}";

            // Build request
            var chatOptions = new ChatCompletionOptions
            {
                Temperature = 0.2f,
                MaxOutputTokenCount = 500
            };

            var messages = new List<ChatMessage>
{
    ChatMessage.CreateSystemMessage(systemPrompt),
    ChatMessage.CreateUserMessage(userPrompt)
};


            var response = await _openAiClient.GetChatClient("gpt-4o-mini")
                .CompleteChatAsync(messages, chatOptions);

            return response.Value?.Content?[0]?.Text ?? "No answer generated.";

        }
    }
}
