using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing.Processors;

namespace AbjjadMicroblogging.Application.Contracts.Services
{
    public class LocalFileStorageService : IStorageService
    {
        private readonly string _storagePath;
        private readonly IImageService _imageProcessor;

        public LocalFileStorageService(IConfiguration configuration, IImageService imageProcessor)
        {
            _storagePath = configuration["LocalFileStorage:Path"] ?? "wwwroot/uploads";
            _imageProcessor = imageProcessor;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), _storagePath);
            Directory.CreateDirectory(directoryPath);

            string baseFileName = Path.GetFileNameWithoutExtension(fileName);

            // Convert the image to WebP
            var webpStream = await _imageProcessor.ConvertToWebPAsync(fileStream);
            string webpFilePath = Path.Combine(directoryPath, $"{baseFileName}.webp");

            using (var outputFileStream = new FileStream(webpFilePath, FileMode.Create, FileAccess.Write))
            {
                await webpStream.CopyToAsync(outputFileStream);
            }

            // Resize and save additional versions
            webpStream.Position = 0; // Reset stream
            var resizedImages = await _imageProcessor.ResizeImageAsync(webpStream, baseFileName);

            foreach (var (resizedStream, resizedFileName) in resizedImages)
            {
                string resizedFilePath = Path.Combine(directoryPath, resizedFileName);

                using (var outputFileStream = new FileStream(resizedFilePath, FileMode.Create, FileAccess.Write))
                {
                    await resizedStream.CopyToAsync(outputFileStream);
                }
            }

            return $"/{_storagePath}/{baseFileName}.webp"; // Return relative URL
        }
    }
}