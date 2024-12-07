namespace AbjjadMicroblogging.Application.Contracts.Services
{
    public interface IStorageServiceFactory
    {
        IStorageService CreateStorageService();
    }
}