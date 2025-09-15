using System.ComponentModel.DataAnnotations;

namespace KnowledgeHub.Api.DTOs
{
    public class DocumentUploadDto
    {
        [Required]
        public IFormFile File { get; set; }

        public Guid? UserId { get; set; }
    }

}
