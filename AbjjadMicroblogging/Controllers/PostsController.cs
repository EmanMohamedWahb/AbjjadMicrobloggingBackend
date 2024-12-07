using AbjjadMicroblogging.API.Models;
using AbjjadMicroblogging.Application.Commands;
using AbjjadMicroblogging.Application.Queries.GetTimeline;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AbjjadMicroblogging.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostCommand command)
        {
            command.UserName = HttpContext.User.Identity.Name;
            var postId = await _mediator.Send(command);
            return Ok(postId);
        }
       
        [HttpGet("{screenSize}")]
        [Authorize]
        public async Task<IActionResult> GetTimeline(int screenSize)
        {
            var posts = await _mediator.Send(new GetTimelineQuery() { ScreenSize=screenSize});
            return Ok(posts);
        }
    }
}
