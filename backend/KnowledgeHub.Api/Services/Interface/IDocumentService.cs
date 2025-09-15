using KnowledgeHub.Api.Models;

namespace KnowledgeHub.Api.Services.Interface
{
    public interface IDocumentService
    {
        Task<Document> UploadDocumentAsync(Guid userId, IFormFile file);
        Task<List<Document>> GetUserDocumentsAsync(Guid userId);
    }
}
