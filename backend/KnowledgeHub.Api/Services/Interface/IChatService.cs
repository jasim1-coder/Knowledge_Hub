using KnowledgeHub.Api.DTOs;

namespace KnowledgeHub.Api.Services.Interface
{
    public interface IChatService
    {
        Task<ChatResponseDto> AskQuestionAsync(ChatRequestDto dto);
        Task<List<ChatDto>> GetUserChatsAsync(Guid userId);
        Task<ChatDto?> GetChatByIdAsync(Guid chatId);
    }

}
