using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using FluentAssertions;
using AbjjadMicroblogging.Application.Commands;
using AbjjadMicroblogging.Application.Contracts.Services;
using AbjjadMicroblogging.Domain;
using AbjjadMicroblogging.Presistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Hangfire;
using System.Linq.Expressions;

public class CreatePostCommandHandlerTests
{
    private readonly Mock<IStorageServiceFactory> _storageServiceFactoryMock;
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly Mock<IImageService> _imageProcessorMock;
    private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
    private readonly CreatePostCommandHandler _handler;

    public CreatePostCommandHandlerTests()
    {
        _storageServiceFactoryMock = new Mock<IStorageServiceFactory>();
        _postRepositoryMock = new Mock<IPostRepository>();
        _imageProcessorMock = new Mock<IImageService>();
        _backgroundJobClientMock = new Mock<IBackgroundJobClient>();

        _handler = new CreatePostCommandHandler(
            _storageServiceFactoryMock.Object,
            _postRepositoryMock.Object,
            _imageProcessorMock.Object,
            _backgroundJobClientMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Post_Without_Image()
    {
        // Arrange
        var command = new CreatePostCommand
        {
            Text = "Test post",
            Latitude = 12.34,
            Longitude = 56.78,
            UserName = "testuser"
        };

        _postRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_Should_Create_Post_With_Image()
    {
        // Arrange
        var mockImageStream = new MemoryStream(new byte[] { 1, 2, 3 });
        var image = new FormFile(mockImageStream, 0, mockImageStream.Length, "image", "test.jpg");
        var command = new CreatePostCommand
        {
            Text = "Post with image",
            Latitude = 12.34,
            Longitude = 56.78,
            UserName = "testuser",
            Image = new FormFile(mockImageStream, 0, mockImageStream.Length, "image", "test.jpg")
        };

        var mockStorageService = new Mock<IStorageService>();
        mockStorageService.Setup(service => service.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("https://storage/image.jpg");

        _storageServiceFactoryMock.Setup(factory => factory.CreateStorageService()).Returns(mockStorageService.Object);
        _postRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(Unit.Value);

        _postRepositoryMock.Verify(repo => repo.AddAsync(It.Is<Post>(p =>
            p.Text == command.Text &&
            p.OriginalImageUrl == "https://storage/image.jpg" &&
            p.Username == command.UserName
        )), Times.Once);

        mockStorageService.Verify(service => service.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
    }
}