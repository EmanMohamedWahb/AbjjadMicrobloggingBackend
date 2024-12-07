using AbjjadMicroblogging.Domain;

namespace AbjjadMicroblogging.Presistence
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUserNameAsync(string userName);
    }
}