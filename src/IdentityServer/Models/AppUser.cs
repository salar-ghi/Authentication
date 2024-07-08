using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models;

public class AppUser : IdentityUser
{
    //public int Id { get; set; }
    public string Name { get; set; }
    public string NationalId { get; set; }
}
