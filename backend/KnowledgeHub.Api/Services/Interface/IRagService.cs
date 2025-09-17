using KnowledgeHub.Api.Models;

namespace KnowledgeHub.Api.Services.Interface
{
    public interface IRagService
    {
        /// <summary>
        /// Generate embeddings for a document section
        /// </summary>
        /// <param name="section">The document section to generate embeddings for</param>
        Task GenerateEmbeddingsAsync(DocumentSection section);

        /// <summary>
        /// Query for the most relevant document sections based on a question
        /// </summary>
        /// <param name="question">The question to search for</param>
        /// <param name="documentIds">Optional list of document IDs to filter by</param>
        /// <param name="topK">Number of top results to return (default: 5)</param>
        /// <returns>List of relevant document sections</returns>
        Task<List<DocumentSection>> QueryRelevantSectionsAsync(
            string question,
            List<Guid>? documentIds = null,
            int topK = 5);

        /// <summary>
        /// Get an AI-powered answer to a question based on relevant documents
        /// </summary>
        /// <param name="userId">The ID of the user asking the question</param>
        /// <param name="question">The question to answer</param>
        /// <param name="documentIds">Optional list of document IDs to search within</param>
        /// <returns>AI-generated answer based on document content</returns>
        Task<string> GetAnswerAsync(Guid userId, string question, List<Guid>? documentIds = null);

        /// <summary>
        /// Check if a document has embeddings generated for all its sections
        /// </summary>
        /// <param name="documentId">The document ID to check</param>
        /// <returns>True if the document has embeddings, false otherwise</returns>
        Task<bool> HasEmbeddingsAsync(Guid documentId);

        /// <summary>
        /// Get the count of document sections that don't have embeddings yet
        /// </summary>
        /// <returns>Number of sections without embeddings</returns>
        Task<int> GetSectionsWithoutEmbeddingsCountAsync();
    }
}