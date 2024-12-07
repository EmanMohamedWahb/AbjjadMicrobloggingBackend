
namespace AbjjadMicroblogging.Application.Contracts.Services
{
    public interface IImageService
    {
        Task<Stream> ConvertToWebPAsync(Stream inputStream);
        Task<List<(Stream Stream, string FileName)>> ResizeImageAsync(Stream inputStream, string baseFileName);
    }
}