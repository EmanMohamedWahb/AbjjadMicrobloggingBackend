using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SixLabors.ImageSharp.Processing.Processors;

namespace AbjjadMicroblogging.Application.Contracts.Services
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly IImageService _imageProcessor;

        public AzureBlobStorageService(IConfiguration configuration, IImageService imageProcessor)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"];
            _containerName = configuration["AzureBlobStorage:ContainerName"];
            _imageProcessor = imageProcessor;
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            await blobContainerClient.CreateIfNotExistsAsync();
            await blobContainerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

            var webpStream = await _imageProcessor.ConvertToWebPAsync(fileStream);
            var webpBlobClient = blobContainerClient.GetBlobClient(fileName);
            await webpBlobClient.UploadAsync(webpStream, new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = "image/webp" });
            fileStream.Dispose();

            return webpBlobClient.Uri.ToString();
        }
    }
}