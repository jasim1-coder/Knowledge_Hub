using KnowledgeHub.Api.Models;

namespace KnowledgeHub.Api.Services.Interface
{
    public interface IDocumentService
    {
        Task<Document> UploadDocumentAsync(Guid userId, IFormFile file);
        Task<List<Document>> GetUserDocumentsAsync(Guid userId);
        Task<Document?> GetDocumentAsync(Guid documentId);
        Task<bool> DeleteDocumentAsync(Guid documentId);
    }
}
