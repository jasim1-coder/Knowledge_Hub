using KnowledgeHub.Api.Data;
using KnowledgeHub.Api.Models;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Embeddings;
using OpenAI.Chat;
using System.Text.Json;

namespace KnowledgeHub.Api.Services
{
    public class RagService : IRagService
    {
        private readonly AppDbContext _context;
        private readonly OpenAIClient _openAI;
        private readonly ILogger<RagService> _logger;

        private const string EMBEDDING_MODEL = "text-embedding-3-small";
        private const string CHAT_MODEL = "gpt-4o-mini";

        public RagService(AppDbContext context, OpenAIClient openAI, ILogger<RagService> logger)
        {
            _context = context;
            _openAI = openAI;
            _logger = logger;
        }

        public async Task GenerateEmbeddingsAsync(DocumentSection section)
        {
            if (string.IsNullOrWhiteSpace(section.Content))
            {
                _logger.LogWarning("Skipping embedding generation for section {SectionId} - empty content", section.Id);
                return;
            }

            try
            {
                var embeddingClient = _openAI.GetEmbeddingClient(EMBEDDING_MODEL);
                var response = await embeddingClient.GenerateEmbeddingAsync(section.Content);

                // Convert ReadOnlyMemory<float> to float array for JSON serialization
                var embeddingVector = response.Value.ToFloats().ToArray();
                section.EmbeddingJson = JsonSerializer.Serialize(embeddingVector);

                _context.DocumentSections.Update(section);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Generated embedding for section {SectionId}", section.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate embedding for section {SectionId}", section.Id);
                throw;
            }
        }

        public async Task<List<DocumentSection>> QueryRelevantSectionsAsync(
            string question,
            List<Guid>? documentIds = null,
            int topK = 5)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                _logger.LogWarning("Empty question provided to QueryRelevantSectionsAsync");
                return new List<DocumentSection>();
            }

            try
            {
                // Generate embedding for the question
                var embeddingClient = _openAI.GetEmbeddingClient(EMBEDDING_MODEL);
                var qResponse = await embeddingClient.GenerateEmbeddingAsync(question);
                var questionVector = qResponse.Value.ToFloats().ToArray();

                // Build query with optional document filtering
                var query = _context.DocumentSections
                    .Include(s => s.Document)
                    .Where(s => !string.IsNullOrEmpty(s.EmbeddingJson));

                if (documentIds?.Any() == true)
                {
                    query = query.Where(s => documentIds.Contains(s.DocumentId));
                }

                var sections = await query.ToListAsync();

                if (!sections.Any())
                {
                    _logger.LogWarning("No sections with embeddings found for query");
                    return new List<DocumentSection>();
                }

                // Calculate similarities and return top results
                var scored = sections
                    .Select(s =>
                    {
                        try
                        {
                            var vec = JsonSerializer.Deserialize<float[]>(s.EmbeddingJson!)!;
                            return new
                            {
                                Section = s,
                                Score = CosineSimilarity(vec, questionVector)
                            };
                        }
                        catch (JsonException ex)
                        {
                            _logger.LogWarning(ex, "Failed to deserialize embedding for section {SectionId}", s.Id);
                            return new
                            {
                                Section = s,
                                Score = 0f
                            };
                        }
                    })
                    .Where(x => x.Score > 0) // Filter out failed deserializations
                    .OrderByDescending(x => x.Score)
                    .Take(topK)
                    .ToList();

                _logger.LogInformation("Found {Count} relevant sections for query with scores: {Scores}",
                    scored.Count, string.Join(", ", scored.Select(s => s.Score.ToString("F3"))));

                return scored.Select(x => x.Section).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying relevant sections");
                throw;
            }
        }

        public async Task<string> GetAnswerAsync(Guid userId, string question, List<Guid>? documentIds = null)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return "Please provide a valid question.";
            }

            try
            {
                _logger.LogInformation("Processing question for user {UserId}: {Question}", userId, question);

                var sections = await QueryRelevantSectionsAsync(question, documentIds);

                if (!sections.Any())
                {
                    return "I don't have any relevant information to answer your question.";
                }

                var contextText = BuildContextText(sections);
                var chatClient = _openAI.GetChatClient(CHAT_MODEL);

                var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(GetSystemPrompt()),
                    new UserChatMessage($"Context:\n{contextText}\n\nQuestion: {question}")
                };

                var chatResponse = await chatClient.CompleteChatAsync(messages);

                var answer = chatResponse.Value.Content[0].Text;
                _logger.LogInformation("Generated answer for user {UserId}", userId);

                return answer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating answer for user {UserId}", userId);
                return "I encountered an error while processing your question. Please try again.";
            }
        }

        private string BuildContextText(List<DocumentSection> sections)
        {
            var contextParts = new List<string>();
            var totalLength = 0;
            const int MAX_CONTEXT_LENGTH = 8000; // Approximate token limit for context

            foreach (var section in sections)
            {
                var sectionText = $"[Document: {section.Document.FileName}]\n{section.Content}";

                // Rough token estimation (1 token ≈ 4 characters)
                if (totalLength + sectionText.Length > MAX_CONTEXT_LENGTH * 4)
                {
                    break;
                }

                contextParts.Add(sectionText);
                totalLength += sectionText.Length;
            }

            return string.Join("\n\n---\n\n", contextParts);
        }

        private static string GetSystemPrompt()
        {
            return @"You are a knowledgeable assistant that answers questions based solely on the provided document context. 

Guidelines:
- Only use information explicitly stated in the provided context
- If the context doesn't contain enough information to answer the question, say 'I don't have enough information to answer that question.'
- Be precise and cite which document the information comes from when relevant
- If multiple documents contain relevant information, synthesize the information clearly
- Maintain a helpful and professional tone";
        }

        private static float CosineSimilarity(float[] vec1, float[] vec2)
        {
            if (vec1.Length != vec2.Length)
            {
                throw new ArgumentException("Vectors must have the same length");
            }

            float dot = 0, mag1 = 0, mag2 = 0;

            for (int i = 0; i < vec1.Length; i++)
            {
                dot += vec1[i] * vec2[i];
                mag1 += vec1[i] * vec1[i];
                mag2 += vec2[i] * vec2[i];
            }

            var denominator = (float)Math.Sqrt(mag1) * (float)Math.Sqrt(mag2);

            // Handle edge case where one vector is zero
            if (denominator == 0)
            {
                return 0;
            }

            return dot / denominator;
        }

        // Additional utility methods
        public async Task<bool> HasEmbeddingsAsync(Guid documentId)
        {
            return await _context.DocumentSections
                .Where(s => s.DocumentId == documentId)
                .AnyAsync(s => !string.IsNullOrEmpty(s.EmbeddingJson));
        }

        public async Task<int> GetSectionsWithoutEmbeddingsCountAsync()
        {
            return await _context.DocumentSections
                .CountAsync(s => string.IsNullOrEmpty(s.EmbeddingJson));
        }
    }
}