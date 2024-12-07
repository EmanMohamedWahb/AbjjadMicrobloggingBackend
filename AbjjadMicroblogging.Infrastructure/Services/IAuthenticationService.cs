
namespace AbjjadMicroblogging.Infrastructure.Services
{
    public interface IAuthenticationService
    {
        Task<string> AuthenticateAsync(string username, string password);
    }
}