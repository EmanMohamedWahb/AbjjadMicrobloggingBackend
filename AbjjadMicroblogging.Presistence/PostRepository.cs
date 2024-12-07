using AbjjadMicroblogging.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbjjadMicroblogging.Presistence
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts.Where(x=>x.Id == id).SingleAsync();
        }

        public async Task UpdatePostImagesAsync(int id, string tabletUrl, string mobileUrl)
        {
            Post post = await _context.Posts.Where(x=>x.Id==id).SingleAsync();
            if (post != null)
            {
                post.ResizedTabletImageUrl = tabletUrl;
                post.ResizedMobileImageUrl = mobileUrl;
            }   
            await _context.SaveChangesAsync();
        }
    }
}
