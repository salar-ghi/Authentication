using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IdentityServer.Infrastructure
{
    public class TokenProvider : ITokenProvider
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public TokenProvider(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //    public async Task<Token> CreateTokenAsync(IEnumerable<Claim> claims)
        //    {
        //        var user = await _userManager.FindFirstAsync(u => u.Email == claims.First(c => c.Type == JwtClaimTypes.Email).Value);

        //        var token = new Token
        //        {
        //            AccessToken = await _signInManager.CreateTokenAsync(user, claims),
        //            ExpiresIn = 3600, // 1 hour
        //            RefreshToken = await _signInManager.CreateRefreshTokenAsync(user)
        //        };

        //        return token;
        //    }
        //}
    }
}

public class Token
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
}