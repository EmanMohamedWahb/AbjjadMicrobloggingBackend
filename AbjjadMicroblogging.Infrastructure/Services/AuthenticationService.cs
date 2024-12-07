using AbjjadMicroblogging.Presistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AbjjadMicroblogging.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthenticationService(IConfiguration configuration, IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {   
            var user = await _userRepository.GetByUserNameAsync(username);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            if (user == null || BCrypt.Net.BCrypt.Verify(user.Password, hashedPassword))
                return null;

            return _jwtTokenService.GenerateToken(username);
        }
    }
}
