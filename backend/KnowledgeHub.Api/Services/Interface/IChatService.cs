namespace KnowledgeHub.Api.Services.Interface
{
    public interface IChatService
    {
        Task<string> AskQuestionAsync(Guid userId, string question, List<Guid>? documentIds = null);

    }
}
