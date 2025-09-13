// Controllers/HealthController.cs
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "OK",
                time = DateTime.UtcNow
            });
        }
    }
}
