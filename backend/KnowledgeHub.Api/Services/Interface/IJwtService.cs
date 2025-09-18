namespace KnowledgeHub.Api.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(string username, IEnumerable<string> roles);
    }

}
