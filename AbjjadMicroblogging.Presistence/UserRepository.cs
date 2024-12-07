using AbjjadMicroblogging.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbjjadMicroblogging.Presistence
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.Where(x => x.Id == id).SingleAsync();
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            return await _context.Users.Where(x => x.Username == userName).SingleOrDefaultAsync();
        }
    }
}
