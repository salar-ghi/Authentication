using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection.Metadata;
using System.Security.Claims;
namespace IdentityServer.Data;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DbContext>();
            context.Database.EnsureDeleted();
            context.Database.Migrate();

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var salar = userMgr.FindByNameAsync("salar").Result;
            if (salar == null)
            {
                salar = new AppUser
                {
                    Name = "Salar",
                    UserName = "salar",
                    Email = "Salarghi@email.com",
                    EmailConfirmed = true,
                    NationalId = "6640009455",
                    PhoneNumber = "09121597532",
                    PhoneNumberConfirmed = true,
                };
                var result = userMgr.CreateAsync(salar, "Pass1234$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(salar, new Claim[]{
                    new Claim(JwtClaimTypes.Name, "Salar ghi"),
                    new Claim(JwtClaimTypes.GivenName, "Salar"),
                    new Claim(JwtClaimTypes.FamilyName, "ghi"),
                    new Claim(JwtClaimTypes.Role, "Admin"),
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("User for salar created");
            }
            else
            {
                Log.Debug("salar already exists");
            }

            var ali = userMgr.FindByNameAsync("ali").Result;
            if (ali == null)
            {
                ali = new AppUser
                {
                    Name = "Ali",
                    UserName = "ali",
                    Email = "aliSmith@email.com",
                    EmailConfirmed = true,
                    NationalId = "6640009455",
                    PhoneNumber = "09129513578",
                    PhoneNumberConfirmed = true,
                };
                var result = userMgr.CreateAsync(ali, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(ali, new Claim[]{
                            new Claim(JwtClaimTypes.Name, "ali Smith"),
                            new Claim(JwtClaimTypes.GivenName, "ali"),
                            new Claim(JwtClaimTypes.FamilyName, "Smith"),
                            new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                            new Claim("location", "somewhere")
                        }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }
        }
    }
}
