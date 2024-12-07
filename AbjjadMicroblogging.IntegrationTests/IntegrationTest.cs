using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AbjjadMicroblogging.API;
using Microsoft.Extensions.DependencyInjection;
using AbjjadMicroblogging.Presistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using AbjjadMicroblogging.Domain;
using AbjjadMicroblogging.API.Controllers;
using AbjjadMicroblogging.Application.Contracts.Services;
using Moq;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Net;
using System;
using Microsoft.AspNetCore.Hosting;

public class IntegrationTest : IClassFixture<WebApplicationFactory<PostsController>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<PostsController> _factory;

    public IntegrationTest(WebApplicationFactory<PostsController> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create the database and seed it with test data
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();

                // Mock other dependencies
                var imageServiceMock = new Mock<IImageService>();
                imageServiceMock.Setup(s => s.ConvertToWebPAsync(It.IsAny<Stream>()))
                                .ReturnsAsync(new MemoryStream());
                imageServiceMock.Setup(s => s.ResizeImageAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                                .ReturnsAsync(new List<(Stream, string)>());
                services.AddSingleton(imageServiceMock.Object);

                var storageServiceMock = new Mock<IStorageService>();
                storageServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>()))
                                  .ReturnsAsync("http://example.com/image.webp");
                services.AddSingleton(storageServiceMock.Object);

                services.AddAuthentication("Test")
                .AddJwtBearer("Test", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("ThisismySecretCustomKeyForTheTechnicalTest"))
                    };
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreatePost_ShouldReturn201Created_WhenPostIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Generate a valid JWT token
        var token = GenerateJwtToken("user1");

        // Add the token to request headers
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var formData = new MultipartFormDataContent
    {
        { new StringContent("This is a test post"), "text" },
        { new StringContent("30.0"), "latitude" },
        { new StringContent("50.0"), "longitude" }
    };

        // Act
        var response = await client.PostAsync("/api/posts", formData);

        // Assert
        response.StatusCode.Equals(HttpStatusCode.Created);
    }


    private string GenerateJwtToken(string username)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ThisismySecretCustomKeyForTheTechnicalTest");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "User")
        }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}