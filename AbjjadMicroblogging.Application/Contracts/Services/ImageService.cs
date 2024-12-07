using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Threading.Tasks;

namespace AbjjadMicroblogging.Application.Contracts.Services
{
    public class ImageService : IImageService
    {
        private static readonly (int Width, int Height)[] ResizeDimensions = new[]
    {
        (1280, 720),  // Tablets and Small Laptops
        (640, 480),   // Mobile Devices
    };
        public async Task<Stream> ConvertToWebPAsync(Stream inputStream)
        {
            using var image = await Image.LoadAsync(inputStream);
            var outputStream = new MemoryStream();
            await image.SaveAsWebpAsync(outputStream);
            outputStream.Position = 0;
            return outputStream;
        }

        public async Task<List<(Stream Stream, string FileName)>> ResizeImageAsync(Stream inputStream, string baseFileName)
        {
            var resizedImages = new List<(Stream, string)>();

            using var image = await Image.LoadAsync(inputStream);

            foreach (var (width, height) in ResizeDimensions)
            {
                var resizedStream = new MemoryStream();
                string fileName = $"{baseFileName}_{width}x{height}.webp";

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Mode = ResizeMode.Max,
                    Size = new Size(width, height)
                }));

                await image.SaveAsWebpAsync(resizedStream);
                resizedStream.Position = 0; // Reset the stream position
                resizedImages.Add((resizedStream, fileName));
            }

            return resizedImages;
        }
    }
}