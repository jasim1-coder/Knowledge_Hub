using Microsoft.AspNetCore.Identity;

namespace KnowledgeHub.Api.Models
{
    public class User : IdentityUser<Guid>
    {


        // Navigation
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    }
}
