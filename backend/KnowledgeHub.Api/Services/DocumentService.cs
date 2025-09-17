using DocumentFormat.OpenXml.Packaging;
using KnowledgeHub.Api.Data;
using KnowledgeHub.Api.Models;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Text;
using UglyToad.PdfPig;

namespace KnowledgeHub.Api.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DocumentService> _logger;

        // Configuration constants
        private const long MaxFileSize = 50 * 1024 * 1024; // 50MB
        private const int DefaultChunkSize = 1000;
        private static readonly string[] AllowedExtensions = { ".pdf", ".docx" };
        private static readonly string[] AllowedContentTypes = {
            "application/pdf",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        public DocumentService(AppDbContext context, IWebHostEnvironment env, ILogger<DocumentService> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        public async Task<Document> UploadDocumentAsync(Guid userId, IFormFile file)
        {
            try
            {
                // Comprehensive validation
                ValidateFile(file);

                // Ensure storage path
                var storagePath = Path.Combine(_env.ContentRootPath, "Storage", "Documents");
                if (!Directory.Exists(storagePath))
                {
                    Directory.CreateDirectory(storagePath);
                    _logger.LogInformation("Created storage directory: {StoragePath}", storagePath);
                }

                // Generate unique file name with timestamp
                var fileExtension = Path.GetExtension(file.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMdd_HHmmss}{fileExtension}";
                var filePath = Path.Combine(storagePath, uniqueFileName);

                // Create document entity with initial status
                var document = new Document
                {
                    UserId = userId,
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileSize = file.Length,
                    ContentType = file.ContentType,
                    Status = DocumentStatus.Processing,
                    IsProcessed = false
                };

                // Save file to disk
                await SaveFileAsync(file, filePath);
                _logger.LogInformation("File saved: {FileName} -> {FilePath}", file.FileName, uniqueFileName);

                // Extract text based on file type
                string extractedText = await ExtractTextAsync(filePath, fileExtension);

                // Split text into manageable sections
                var sections = CreateDocumentSections(extractedText);

                // Update document with processed data
                document.Sections = sections;
                document.Status = DocumentStatus.Processed;
                document.IsProcessed = true;
                document.LastProcessedAt = DateTime.UtcNow;

                // Save to database
                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document processed successfully: {DocumentId}, Sections: {SectionCount}",
                    document.Id, sections.Count);

                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document: {FileName}", file?.FileName ?? "Unknown");

                // Update status to failed if document was created
                // Note: In a real scenario, you might want to handle this differently
                throw;
            }
        }

        public async Task<List<Document>> GetUserDocumentsAsync(Guid userId)
        {
            try
            {
                var documents = await _context.Documents
                    .Where(d => d.UserId == userId)
                    .Include(d => d.Sections)
                    .OrderByDescending(d => d.UploadedAt)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} documents for user {UserId}", documents.Count, userId);
                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for user {UserId}", userId);
                throw;
            }
        }

        public async Task<Document?> GetDocumentAsync(Guid documentId)
        {
            try
            {
                var document = await _context.Documents
                    .Include(d => d.Sections)
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<bool> DeleteDocumentAsync(Guid documentId)
        {
            try
            {
                var document = await _context.Documents
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                if (document == null)
                    return false;

                // Delete physical file
                if (File.Exists(document.FilePath))
                {
                    File.Delete(document.FilePath);
                    _logger.LogInformation("Deleted file: {FilePath}", document.FilePath);
                }

                // Delete from database
                _context.Documents.Remove(document);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document deleted: {DocumentId}", documentId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document {DocumentId}", documentId);
                throw;
            }
        }

        #region Private Methods

        private static void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided or file is empty");

            if (file.Length > MaxFileSize)
                throw new ArgumentException($"File size exceeds the maximum limit of {MaxFileSize / (1024 * 1024)}MB");

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
                throw new ArgumentException($"Unsupported file type. Allowed types: {string.Join(", ", AllowedExtensions)}");

            if (!AllowedContentTypes.Contains(file.ContentType))
                throw new ArgumentException($"Invalid content type: {file.ContentType}");
        }

        private static async Task SaveFileAsync(IFormFile file, string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        private async Task<string> ExtractTextAsync(string filePath, string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".pdf" => await ExtractTextFromPdfAsync(filePath),
                ".docx" => await ExtractTextFromDocxAsync(filePath),
                _ => throw new ArgumentException($"Unsupported file extension: {extension}")
            };
        }

        private async Task<string> ExtractTextFromPdfAsync(string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var sb = new StringBuilder();
                    using var pdf = PdfDocument.Open(filePath);

                    foreach (var page in pdf.GetPages())
                    {
                        var pageText = page.Text?.Trim();
                        if (!string.IsNullOrEmpty(pageText))
                        {
                            sb.AppendLine(pageText);
                        }
                    }

                    return sb.ToString();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from PDF: {FilePath}", filePath);
                throw new InvalidOperationException("Failed to extract text from PDF", ex);
            }
        }

        private async Task<string> ExtractTextFromDocxAsync(string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var sb = new StringBuilder();

                    using var wordDoc = WordprocessingDocument.Open(filePath, false);
                    var mainPart = wordDoc.MainDocumentPart;

                    if (mainPart?.Document?.Body == null)
                        return string.Empty;

                    // Extract all text nodes while preserving order
                    var textNodes = mainPart.Document.Body
                        .Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>();

                    foreach (var textNode in textNodes)
                    {
                        if (!string.IsNullOrEmpty(textNode.Text))
                        {
                            sb.Append(textNode.Text);
                            sb.Append(' ');
                        }
                    }

                    return sb.ToString().Trim();
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting text from DOCX: {FilePath}", filePath);
                throw new InvalidOperationException("Failed to extract text from DOCX", ex);
            }
        }

        private static List<DocumentSection> CreateDocumentSections(string text, int chunkSize = DefaultChunkSize)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<DocumentSection>();

            var sections = new List<DocumentSection>();

            // Split by sentences for better semantic chunking
            var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(s => s.Trim())
                               .Where(s => !string.IsNullOrEmpty(s))
                               .ToArray();

            if (sentences.Length == 0)
            {
                // Fallback to character-based chunking if no sentence boundaries found
                return CreateCharacterBasedSections(text, chunkSize);
            }

            var currentChunk = new StringBuilder();
            int order = 1;

            foreach (var sentence in sentences)
            {
                var sentenceWithPunctuation = sentence + ".";

                // Check if adding this sentence would exceed chunk size
                if (currentChunk.Length > 0 &&
                    currentChunk.Length + sentenceWithPunctuation.Length + 1 > chunkSize)
                {
                    // Save current chunk
                    sections.Add(new DocumentSection
                    {
                        Content = currentChunk.ToString().Trim(),
                        Order = order++
                    });
                    currentChunk.Clear();
                }

                // Add sentence to current chunk
                if (currentChunk.Length > 0)
                    currentChunk.Append(' ');
                currentChunk.Append(sentenceWithPunctuation);
            }

            // Add remaining content
            if (currentChunk.Length > 0)
            {
                sections.Add(new DocumentSection
                {
                    Content = currentChunk.ToString().Trim(),
                    Order = order
                });
            }

            return sections;
        }

        private static List<DocumentSection> CreateCharacterBasedSections(string text, int chunkSize)
        {
            var sections = new List<DocumentSection>();
            int order = 1;

            for (int i = 0; i < text.Length; i += chunkSize)
            {
                var remainingLength = text.Length - i;
                var actualChunkSize = Math.Min(chunkSize, remainingLength);
                var chunk = text.Substring(i, actualChunkSize);

                sections.Add(new DocumentSection
                {
                    Content = chunk.Trim(),
                    Order = order++
                });
            }

            return sections;
        }

        #endregion
    }
}