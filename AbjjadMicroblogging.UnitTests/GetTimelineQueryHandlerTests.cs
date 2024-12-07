using AbjjadMicroblogging.Application.Queries.GetTimeline;
using AbjjadMicroblogging.Domain;
using AbjjadMicroblogging.Presistence;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class GetTimelineQueryHandlerTests
{
    private readonly Mock<IPostRepository> _postRepositoryMock;
    private readonly GetTimelineQueryHandler _handler;

    public GetTimelineQueryHandlerTests()
    {
        _postRepositoryMock = new Mock<IPostRepository>();
        _handler = new GetTimelineQueryHandler(_postRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPostsOrderedByCreatedAt_WithCorrectImageUrls_ForMobileScreen()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post
            {
                Id = 1,
                Text = "Post 1",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Username = "User1",
                OriginalImageUrl = "original1.jpg",
                ResizedMobileImageUrl = "mobile1.jpg",
                ResizedTabletImageUrl = "tablet1.jpg"
            },
            new Post
            {
                Id = 2,
                Text = "Post 2",
                CreatedAt = DateTime.UtcNow,
                Username = "User2",
                OriginalImageUrl = "original2.jpg",
                ResizedMobileImageUrl = "mobile2.jpg",
                ResizedTabletImageUrl = "tablet2.jpg"
            }
        };

        _postRepositoryMock.Setup(repo => repo.GetAllAsync())
                           .ReturnsAsync(posts);

        var query = new GetTimelineQuery { ScreenSize = 768 }; // Mobile screen

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Post 2", result[0].Text); // Latest post first
        Assert.Equal("mobile2.jpg", result[0].ImageUrl); // Mobile image
        Assert.Equal("mobile1.jpg", result[1].ImageUrl); // Mobile image
    }

    [Fact]
    public async Task Handle_ShouldReturnPostsOrderedByCreatedAt_WithCorrectImageUrls_ForTabletScreen()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post
            {
                Id = 1,
                Text = "Post 1",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Username = "User1",
                OriginalImageUrl = "original1.jpg",
                ResizedMobileImageUrl = "mobile1.jpg",
                ResizedTabletImageUrl = "tablet1.jpg"
            },
            new Post
            {
                Id = 2,
                Text = "Post 2",
                CreatedAt = DateTime.UtcNow,
                Username = "User2",
                OriginalImageUrl = "original2.jpg",
                ResizedMobileImageUrl = "mobile2.jpg",
                ResizedTabletImageUrl = "tablet2.jpg"
            }
        };

        _postRepositoryMock.Setup(repo => repo.GetAllAsync())
                           .ReturnsAsync(posts);

        var query = new GetTimelineQuery { ScreenSize = 1366 }; // Tablet screen

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("tablet2.jpg", result[0].ImageUrl); // Tablet image
        Assert.Equal("tablet1.jpg", result[1].ImageUrl); // Tablet image
    }

    [Fact]
    public async Task Handle_ShouldReturnPostsOrderedByCreatedAt_WithCorrectImageUrls_ForDesktopScreen()
    {
        // Arrange
        var posts = new List<Post>
        {
            new Post
            {
                Id = 1,
                Text = "Post 1",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                Username = "User1",
                OriginalImageUrl = "original1.jpg",
                ResizedMobileImageUrl = "mobile1.jpg",
                ResizedTabletImageUrl = "tablet1.jpg"
            },
            new Post
            {
                Id = 2,
                Text = "Post 2",
                CreatedAt = DateTime.UtcNow,
                Username = "User2",
                OriginalImageUrl = "original2.jpg",
                ResizedMobileImageUrl = "mobile2.jpg",
                ResizedTabletImageUrl = "tablet2.jpg"
            }
        };

        _postRepositoryMock.Setup(repo => repo.GetAllAsync())
                           .ReturnsAsync(posts);

        var query = new GetTimelineQuery { ScreenSize = 1920 }; // Desktop screen

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("original2.jpg", result[0].ImageUrl); // Original image
        Assert.Equal("original1.jpg", result[1].ImageUrl); // Original image
    }
}