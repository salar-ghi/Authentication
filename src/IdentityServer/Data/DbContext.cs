using IdentityServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer.Data;

public class DbContext : IdentityDbContext<AppUser>
{
    public DbContext(DbContextOptions<DbContext> options) :  base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }


}
