namespace Server.Services
{
    public interface ITokenService
    {
        // Declaração do método para gerar token
        string GenerateToken(string userId, string userEmail);
    }
}