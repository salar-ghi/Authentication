using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using IdentityServer.Dtos;
using IdentityServer.Infrastructure;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IEventService _events;
    private readonly IAuthenticationSchemeProvider _schemeProvider;
    private readonly IIdentityProviderStore _identityProviderStore;


    public AccountController(
        UserManager<AppUser> userManager, 
        SignInManager<AppUser> signInManager,
        IIdentityServerInteractionService interaction,
        IAuthenticationSchemeProvider schemeProvider,
        IIdentityProviderStore identityProviderStore,
        IEventService events)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _interaction = interaction;
    }

    [HttpPost("register")]
    [SecurityHeaders]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var context = await _interaction.GetAuthorizationContextAsync(dto.ReturnUrl);
        var user = new AppUser
        {
            Name = dto.Name,
            UserName = dto.Email,
            Email = dto.Email,
            EmailConfirmed = true,
            NationalId  = dto.NationalId,
            PhoneNumber = dto.PhoneNumber,
            PhoneNumberConfirmed = true,
        };

        if (_userManager.FindByEmailAsync(dto.Email) != null)
        {
            ModelState.AddModelError("Input.Username", "Invalid username");
            return BadRequest($"{dto.Email}, Invalid Username");
        }
        if (ModelState.IsValid)
        {

        }


        var result = await _userManager.CreateAsync(user, dto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.AddClaimsAsync(user, new Claim[] 
            {
                new Claim("role", "User"),
                new Claim("email", dto.Email)
            });
            
            var sub = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);
            var issuer = new IdentityServerUser(sub);

            await HttpContext.SignInAsync(issuer);

            return Ok(new { message = "User registered successfully" });
        }
        return BadRequest(result.Errors);
    }



    public async Task<IActionResult> Login(LoginDto dto)
    {
        var context = await _interaction.GetAuthorizationContextAsync(dto.ReturnUrl);
        var user = await _userManager.FindByEmailAsync(dto.Email);
        var sub = CryptoRandom.CreateUniqueId(format: CryptoRandom.OutputFormat.Hex);
        await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName,sub, user.UserName, clientId: context?.Client.ClientId));
        var result = await _signInManager.PasswordSignInAsync(user, dto.Password, false, true);
        if (result.Succeeded)
        {
            //var claims = new List<Claim>
            //    new Claim(JwtClaimTypes.Name, user.UserName),
            //    new Claim(JwtClaimTypes.Email, user.Email),
            //    new Claim(JwtClaimTypes.Role, user.NormalizedUserName)
            //};

            return Ok();
        }

        return Unauthorized();
    }
    
}
