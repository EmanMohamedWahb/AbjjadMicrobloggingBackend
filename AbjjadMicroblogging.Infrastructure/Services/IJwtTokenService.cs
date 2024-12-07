using System.Security.Claims;

namespace AbjjadMicroblogging.Infrastructure.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username);
        ClaimsPrincipal GetPrincipal(string token);
        string ValidateToken(string token);
    }
}