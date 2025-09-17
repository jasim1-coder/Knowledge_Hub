using KnowledgeHub.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KnowledgeHub.Api.DTOs
{
    // Document Upload DTO
    public class DocumentUploadDto
    {
        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; } = null!;

        public Guid? UserId { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }

    // Document Response DTO
    public class DocumentResponseDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime UploadedAt { get; set; }
        public DateTime? LastProcessedAt { get; set; }
        public bool IsProcessed { get; set; }
        public DocumentStatus Status { get; set; }
        public Guid UserId { get; set; }
        public int SectionCount { get; set; }
    }

    // Document with Sections DTO
    public class DocumentWithSectionsDto : DocumentResponseDto
    {
        public List<DocumentSectionDto> Sections { get; set; } = new List<DocumentSectionDto>();
    }

    // Document Section DTO
    public class DocumentSectionDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    // Document Query Parameters DTO
    public class DocumentQueryDto
    {
        [FromQuery]
        public int Page { get; set; } = 1;

        [FromQuery]
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        [FromQuery]
        public DocumentStatus? Status { get; set; }

        [FromQuery]
        public string? FileName { get; set; }

        [FromQuery]
        public DateTime? UploadedAfter { get; set; }

        [FromQuery]
        public DateTime? UploadedBefore { get; set; }
    }

    // Paginated Response DTO
    public class PaginatedResponseDto<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

    // API Response Wrapper
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> ErrorResponse(string error, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = data,
                Errors = new List<string> { error }
            };
        }

        public static ApiResponse<T> ErrorResponse(List<string> errors, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = data,
                Errors = errors
            };
        }
    }

    // Document Search DTO
    public class DocumentSearchDto
    {
        [Required(ErrorMessage = "Search query is required")]
        [MinLength(2, ErrorMessage = "Search query must be at least 2 characters")]
        public string Query { get; set; } = string.Empty;

        public Guid? UserId { get; set; }

        [Range(1, 100, ErrorMessage = "Limit must be between 1 and 100")]
        public int Limit { get; set; } = 10;

        public DocumentStatus? Status { get; set; }
    }

    // Document Section Search Result DTO
    public class DocumentSectionSearchResultDto
    {
        public Guid SectionId { get; set; }
        public Guid DocumentId { get; set; }
        public string DocumentFileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public double RelevanceScore { get; set; }
        public string MatchedText { get; set; } = string.Empty;
    }

    // Document Statistics DTO
    public class DocumentStatisticsDto
    {
        public int TotalDocuments { get; set; }
        public int ProcessedDocuments { get; set; }
        public int ProcessingDocuments { get; set; }
        public int FailedDocuments { get; set; }
        public long TotalFileSize { get; set; }
        public int TotalSections { get; set; }
        public DateTime? LastUploadDate { get; set; }
        public string FormattedTotalSize => FormatBytes(TotalFileSize);

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}


