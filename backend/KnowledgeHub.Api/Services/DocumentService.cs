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

        public DocumentService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<Document> UploadDocumentAsync(Guid userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid file upload");

            // Ensure storage path
            var storagePath = Path.Combine(_env.ContentRootPath, "Storage", "Documents");
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);

            // Unique file name
            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(storagePath, uniqueFileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Extract text based on file type
            string extractedText;
            if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                extractedText = ExtractTextFromPdf(filePath);
            }
            else if (file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            {
                extractedText = ExtractTextFromDocx(filePath);
            }
            else
            {
                extractedText = "[Unsupported file format]";
            }

            // Split text into 1000-char chunks
            var sections = SplitIntoSections(extractedText, 1000);

            var document = new Document
            {
                UserId = userId,
                FileName = file.FileName,
                FilePath = filePath,
                Sections = sections
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            return document;
        }

        private string ExtractTextFromPdf(string filePath)
        {
            var sb = new StringBuilder();
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages())
                {
                    sb.AppendLine(page.Text);
                }
            }
            return sb.ToString();
        }

        private string ExtractTextFromDocx(string filePath)
        {
            var sb = new StringBuilder();

            using (var wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var mainPart = wordDoc.MainDocumentPart;
                if (mainPart?.Document?.Body == null) return string.Empty;

                // Pull all text nodes (keeps words in order)
                foreach (var t in mainPart.Document.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                {
                    sb.Append(t.Text);
                    sb.Append(' ');
                }
            }

            return sb.ToString().Trim();
        }

        private List<DocumentSection> SplitIntoSections(string text, int chunkSize)
        {
            var sections = new List<DocumentSection>();
            int order = 1;

            for (int i = 0; i < text.Length; i += chunkSize)
            {
                var chunk = text.Substring(i, Math.Min(chunkSize, text.Length - i));
                sections.Add(new DocumentSection
                {
                    Content = chunk,
                    Order = order++
                });
            }

            return sections;
        }


        public async Task<List<Document>> GetUserDocumentsAsync(Guid userId)
        {
            return await _context.Documents
                .Where(d => d.UserId == userId)
                .Include(d => d.Sections)
                .ToListAsync();
        }
    }
}
