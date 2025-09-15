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
        public async Task<IActionResult> AskQuestion([FromQuery] Guid userId, [FromBody] string question)
        {
            var answer = await _chatService.AskQuestionAsync(userId, question);
            return Ok(new { Answer = answer });
        }
    }

}
