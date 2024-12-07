using AbjjadMicroblogging.Application.Contracts.Services;
using AbjjadMicroblogging.Domain;
using AbjjadMicroblogging.Presistence;
using Azure.Core;
using Azure.Storage.Blobs;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbjjadMicroblogging.Application.Commands
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, Unit>
    {
        private readonly IStorageServiceFactory _storageServiceFactory;
        private readonly IPostRepository _postRepository;
        private readonly IImageService _imageProcessor;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreatePostCommandHandler(IStorageServiceFactory storageServiceFactory, IPostRepository postRepository,
            IImageService imageProcessor, IBackgroundJobClient backgroundJobClient)
        {
            _storageServiceFactory = storageServiceFactory;
            _postRepository = postRepository;
            _imageProcessor = imageProcessor;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Unit> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            string imageUrl = "";
            string fileName = "";
            var storageService = _storageServiceFactory.CreateStorageService();

            if (request.Image != null && request.Image.Length > 0)
            {
                using var stream = new MemoryStream();
                await request.Image.CopyToAsync(stream);
                stream.Position = 0;
                
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";

                imageUrl = await storageService.UploadFileAsync(stream, fileName);
            }

            var post = new Post
            {
                Text = request.Text,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                OriginalImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                Username = request.UserName
            };

            await _postRepository.AddAsync(post);

            if (request.Image != null && request.Image.Length > 0)
            {
                if (!Directory.Exists("uploads"))
                    Directory.CreateDirectory("uploads");

                string filePath = Path.Combine("uploads", fileName);
                using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(fileStream);
                }
                _backgroundJobClient.Enqueue(() => ProcessImage(post.Id, filePath));
            }
            return Unit.Value;
        }
        public async Task ProcessImage(int postId, string fileName)
        {
            FileStream image = new FileStream(fileName, FileMode.Open,FileAccess.ReadWrite);
            using var stream = new MemoryStream();
            await image.CopyToAsync(stream);
            stream.Position = 0;

            var webpStream = await _imageProcessor.ConvertToWebPAsync(stream);
            var resizedImages = await _imageProcessor.ResizeImageAsync(webpStream, Path.GetFileNameWithoutExtension(fileName));

            string ResizedMobileImageUrl = "";
            string ResizedTabletImageUrl = "";
            var storageService = _storageServiceFactory.CreateStorageService();

            foreach (var (resizedStream, resizedFileName) in resizedImages)
            {
                string imageUrl = await storageService.UploadFileAsync(resizedStream, resizedFileName);
                string imageWidth = resizedFileName.Split("_")[1].Split("x")[0];
                if (int.Parse(imageWidth) <= 768)
                    ResizedMobileImageUrl = imageUrl;
                else ResizedTabletImageUrl = imageUrl;
            }
            stream.Dispose();
            image.Dispose();
            await _postRepository.UpdatePostImagesAsync(postId, ResizedTabletImageUrl, ResizedMobileImageUrl);
            File.Delete(fileName);   
        }
    }
}
