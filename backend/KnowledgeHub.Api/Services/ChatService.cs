using KnowledgeHub.Api.Data;
using KnowledgeHub.Api.DTOs;
using KnowledgeHub.Api.Models;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace KnowledgeHub.Api.Services
{
    public class ChatService : IChatService
    {
        private readonly AppDbContext _context;
        private readonly IRagService _ragService;

        public ChatService(AppDbContext context, IRagService ragService)
        {
            _context = context;
            _ragService = ragService;
        }

        public async Task<ChatResponseDto> AskQuestionAsync(ChatRequestDto dto)
        {
            // 1. Get answer from RAG service (embeddings, retrieval, prompt building)
            var answer = await _ragService.GetAnswerAsync(dto.UserId, dto.Question, dto.DocumentIds);

            // 2. Save chat and messages
            var chat = new Chat
            {
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Chats.Add(chat);

            // Save user question
            _context.Messages.Add(new Message
            {
                Chat = chat,
                Role = "user",
                Content = dto.Question,
                SentAt = DateTime.UtcNow
            });

            // Save assistant answer
            _context.Messages.Add(new Message
            {
                Chat = chat,
                Role = "assistant",
                Content = answer,
                SentAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // 3. Return response
            return new ChatResponseDto
            {
                Answer = answer,
                Sources = new List<string>() // optionally, you can include document/chunk IDs if needed
            };


        }

        // 🔹 Get all chats of a user with messages
        public async Task<List<ChatDto>> GetUserChatsAsync(Guid userId)
        {
            return await _context.Chats
                .Where(c => c.UserId == userId)
                .Include(c => c.Messages)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ChatDto
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    Messages = c.Messages
                        .OrderBy(m => m.SentAt)
                        .Select(m => new MessageDto
                        {
                            Id = m.Id,
                            Role = m.Role,
                            Content = m.Content,
                            SentAt = m.SentAt
                        }).ToList()
                })
                .ToListAsync();
        }

        // 🔹 Get a single chat with messages
        public async Task<ChatDto?> GetChatByIdAsync(Guid chatId)
        {
            return await _context.Chats
                .Where(c => c.Id == chatId)
                .Include(c => c.Messages)
                .Select(c => new ChatDto
                {
                    Id = c.Id,
                    CreatedAt = c.CreatedAt,
                    Messages = c.Messages
                        .OrderBy(m => m.SentAt)
                        .Select(m => new MessageDto
                        {
                            Id = m.Id,
                            Role = m.Role,
                            Content = m.Content,
                            SentAt = m.SentAt
                        }).ToList()
                })
                .FirstOrDefaultAsync();

        }
    }
}