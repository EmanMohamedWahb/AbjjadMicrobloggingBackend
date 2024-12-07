using AbjjadMicroblogging.Application.Contracts.Services;
using AbjjadMicroblogging.Domain;
using AbjjadMicroblogging.Presistence;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbjjadMicroblogging.Application.Queries.GetTimeline
{
    public class GetTimelineQueryHandler : IRequestHandler<GetTimelineQuery, List<PostDto>>
    {
        private readonly IPostRepository _postRepository;

        public GetTimelineQueryHandler(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<List<PostDto>> Handle(GetTimelineQuery request, CancellationToken cancellationToken)
        {
            var posts = await _postRepository.GetAllAsync();

            return posts.OrderByDescending(p => p.CreatedAt)
                        .Select(p => new PostDto
                        {
                            Id = p.Id,
                            Text = p.Text,
                            ImageUrl = (request.ScreenSize <= 768)? p.ResizedMobileImageUrl:(request.ScreenSize <= 1366)?p.ResizedTabletImageUrl: p.OriginalImageUrl,
                            CreatedAt = p.CreatedAt,
                            Username = p.Username
                        }).ToList();
        }
    }
}
