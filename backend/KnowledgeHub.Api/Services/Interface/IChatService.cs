using KnowledgeHub.Api.DTOs;

namespace KnowledgeHub.Api.Services.Interface
{
    public interface IChatService
    {
        Task<ChatResponseDto> AskQuestionAsync(ChatRequestDto dto);
    }

}
