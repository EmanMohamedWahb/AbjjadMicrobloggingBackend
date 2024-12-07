using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbjjadMicroblogging.Application.Queries.GetTimeline
{
    public class GetTimelineQuery : IRequest<List<PostDto>> {
        public int ScreenSize { get; set; }
    }
}
