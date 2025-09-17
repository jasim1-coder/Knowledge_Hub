using KnowledgeHub.Api.DTOs;
using KnowledgeHub.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatRequestDto dto)
        {
            var response = await _chatService.AskQuestionAsync(dto);
            return Ok(response);
        }

    }

}
