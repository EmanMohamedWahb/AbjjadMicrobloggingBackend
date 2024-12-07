using MediatR;
using Microsoft.AspNetCore.Http;

namespace AbjjadMicroblogging.Application.Commands
{
    public class CreatePostCommand : IRequest<Unit>
    {
        public string Text { get; set; }
        public string? UserName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IFormFile? Image { get; set; }
    }
}
