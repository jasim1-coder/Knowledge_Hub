using KnowledgeHub.Api.DTOs;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] DocumentUploadDto dto)
        {
            if (dto.File == null)
                return BadRequest("No file provided");

            // Use provided UserId or generate a new one
            var userId = dto.UserId ?? Guid.NewGuid();

            // Upload document
            var doc = await _documentService.UploadDocumentAsync(userId, dto.File);

            return Ok(new
            {
                doc.Id,
                doc.FileName,
                doc.FilePath,
                doc.UserId
            });
        }

        //[HttpGet("user/{userId}")]
        //public async Task<IActionResult> GetUserDocuments(Guid userId)
        //{
        //    var docs = await _documentService.GetUserDocumentsAsync(userId);
        //    return Ok(docs);
        //}


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDocuments(Guid userId)
        {
            var docs = await _documentService.GetUserDocumentsAsync(userId);

            var response = docs.Select(d => new
            {
                d.Id,
                d.FileName,
                d.FilePath,
                d.UserId,
                Sections = d.Sections.Select(s => new
                {
                    s.Id,
                    s.Content,
                    
                })
            });

            return Ok(response);
        }

    }
}
