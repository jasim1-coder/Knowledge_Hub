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
            try
            {
                if (dto.File == null)
                    return BadRequest(new { error = "No file provided" });

                var userId = dto.UserId ?? Guid.NewGuid();
                var doc = await _documentService.UploadDocumentAsync(userId, dto.File);

                return CreatedAtAction(nameof(GetUserDocuments),
                    new { userId = doc.UserId },
                    new
                    {
                        doc.Id,
                        doc.FileName,
                        doc.FileSize,
                        doc.Status,
                        doc.UserId,
                        SectionCount = doc.Sections.Count
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "An error occurred while processing the document" });
            }
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
