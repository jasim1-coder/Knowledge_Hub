using KnowledgeHub.Api.Models;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KnowledgeHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RagController : ControllerBase
    {
        private readonly IRagService _ragService;
        private readonly ILogger<RagController> _logger;

        public RagController(IRagService ragService, ILogger<RagController> logger)
        {
            _ragService = ragService;
            _logger = logger;
        }

        /// <summary>
        /// Ask a question and get an AI-powered answer from your documents
        /// </summary>
        [HttpPost("ask")]
        public async Task<ActionResult<AskQuestionResponse>> AskQuestion([FromBody] AskQuestionRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var answer = await _ragService.GetAnswerAsync(
                    request.UserId,
                    request.Question,
                    request.DocumentIds
                );

                var response = new AskQuestionResponse
                {
                    Answer = answer,
                    ProcessedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Successfully processed question for user {UserId}", request.UserId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing question for user {UserId}", request.UserId);
                return StatusCode(500, new { error = "An error occurred while processing your question." });
            }
        }

        /// <summary>
        /// Find the most relevant document sections for a given question
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchResponse>> SearchRelevantSections([FromBody] SearchRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sections = await _ragService.QueryRelevantSectionsAsync(
                    request.Query,
                    request.DocumentIds,
                    request.TopK ?? 5
                );

                var response = new SearchResponse
                {
                    Sections = sections.Select(s => new SectionResult
                    {
                        Id = s.Id,
                        Content = s.Content,
                        DocumentId = s.DocumentId,
                        DocumentName = s.Document.FileName,
                        // Note: You'd need to calculate similarity score if you want to return it
                    }).ToList(),
                    TotalFound = sections.Count,
                    SearchedAt = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching sections for query: {Query}", request.Query);
                return StatusCode(500, new { error = "An error occurred while searching." });
            }
        }

        /// <summary>
        /// Generate embeddings for a specific document section
        /// </summary>
        [HttpPost("generate-embeddings/document/{documentId}")]
        public async Task<ActionResult> GenerateDocumentEmbeddings(Guid documentId)
        {
            try
            {
                var processedSections = await _ragService.GenerateEmbeddingsForDocumentAsync(documentId);

                if (processedSections == 0)
                    return Ok(new { message = "All sections already have embeddings." });

                return Ok(new
                {
                    message = "Embeddings generated for document sections.",
                    documentId,
                    processedSections
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embeddings for document {DocumentId}", documentId);
                return StatusCode(500, new { error = "An error occurred while generating embeddings." });
            }
        }


        /// <summary>
        /// Check if a document has embeddings generated
        /// </summary>
        [HttpGet("document/{documentId}/embeddings-status")]
        public async Task<ActionResult<EmbeddingStatusResponse>> GetEmbeddingStatus(Guid documentId)
        {
            try
            {
                var hasEmbeddings = await _ragService.HasEmbeddingsAsync(documentId);

                var response = new EmbeddingStatusResponse
                {
                    DocumentId = documentId,
                    HasEmbeddings = hasEmbeddings,
                    CheckedAt = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking embedding status for document {DocumentId}", documentId);
                return StatusCode(500, new { error = "An error occurred while checking embedding status." });
            }
        }

        /// <summary>
        /// Get count of sections without embeddings
        /// </summary>
        [HttpGet("embeddings/pending-count")]
        public async Task<ActionResult<PendingEmbeddingsResponse>> GetPendingEmbeddingsCount()
        {
            try
            {
                var count = await _ragService.GetSectionsWithoutEmbeddingsCountAsync();

                var response = new PendingEmbeddingsResponse
                {
                    PendingCount = count,
                    CheckedAt = DateTime.UtcNow
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending embeddings count");
                return StatusCode(500, new { error = "An error occurred while checking pending embeddings." });
            }
        }
        public class AskQuestionRequest
        {
            [Required]
            [MinLength(1)]
            public string Question { get; set; } = string.Empty;

            [Required]
            public Guid UserId { get; set; }

            public List<Guid>? DocumentIds { get; set; }
        }

        public class AskQuestionResponse
        {
            public string Answer { get; set; } = string.Empty;
            public DateTime ProcessedAt { get; set; }
        }

        public class SearchRequest
        {
            [Required]
            [MinLength(1)]
            public string Query { get; set; } = string.Empty;

            public List<Guid>? DocumentIds { get; set; }

            [Range(1, 50)]
            public int? TopK { get; set; } = 5;
        }

        public class SearchResponse
        {
            public List<SectionResult> Sections { get; set; } = new();
            public int TotalFound { get; set; }
            public DateTime SearchedAt { get; set; }
        }

        public class SectionResult
        {
            public Guid Id { get; set; }
            public string Content { get; set; } = string.Empty;
            public Guid DocumentId { get; set; }
            public string DocumentName { get; set; } = string.Empty;
            public float? SimilarityScore { get; set; }
        }

        public class EmbeddingStatusResponse
        {
            public Guid DocumentId { get; set; }
            public bool HasEmbeddings { get; set; }
            public DateTime CheckedAt { get; set; }
        }

        public class PendingEmbeddingsResponse
        {
            public int PendingCount { get; set; }
            public DateTime CheckedAt { get; set; }
        }
    }
}