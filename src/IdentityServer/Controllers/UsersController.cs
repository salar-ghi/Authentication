using IdentityServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private static readonly List<AppUser> users = new()
        {
            new AppUser { Id = Guid.NewGuid().ToString() , NationalId = "5260426304", Name = "John Doe", Email = "john@example.com" },
            new AppUser { Id = Guid.NewGuid().ToString(), NationalId = "5260426304", Name = "Jane Smith", Email = "jane@example.com" },
            new AppUser { Id = Guid.NewGuid().ToString(), NationalId = "5260426304", Name = "Bob Johnson", Email = "bob@example.com" },
            new AppUser { Id = Guid.NewGuid().ToString(), NationalId = "5260426304", Name = "Alice Williams", Email = "alice@example.com" },
            new AppUser { Id = Guid.NewGuid().ToString(), NationalId = "5260426304", Name = "Tom Brown", Email = "tom@example.com" }
    };

    [HttpGet]
    public IEnumerable<AppUser> GetUsers()
    {
        return users;
    }




}