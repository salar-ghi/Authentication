using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using IdentityServer.Data;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

namespace IdentityServer.Infrastructure;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
        const string connectionString = @"Server=.;Database=IdentityTest;User ID=sa;Password=Nitro912*;MultipleActiveResultSets=true;TrustServerCertificate=True";
        builder.Services.AddDbContext<IdentityServer.Data.DbContext>(options =>
                options.UseSqlServer(connectionString));

        builder.Services.AddControllers();

        builder.Services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<IdentityServer.Data.DbContext>()
            .AddDefaultTokenProviders();
        // uncomment if you want to add a UI
        //builder.Services.AddRazorPages();

        // Configure IdentityServer
        builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/api_scopes#authorization-based-on-scopes
                options.EmitStaticAudienceClaim = true;
            })
            .AddAspNetIdentity<AppUser>()
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryClients(Config.Clients)
            .AddDeveloperSigningCredential();

        //builder.Services.AddAuthorization();

        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(options =>
        //    {
        //        //options.TokenValidationParameters = new TokenValidationParameters
        //        //{
        //        //    ValidateIssuer = true,
        //        //    ValidateAudience = true,
        //        //    ValidateLifetime = true,
        //        //    ValidateIssuerSigningKey = true,
        //        //    ValidIssuer = builder.Configuration["Jwt:Issuer"],
        //        //    ValidAudience = builder.Configuration["Jwt:Audience"],
        //        //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        //        //};
        //    })
        //    .AddCookie("Cookies")
        //    .AddOpenIdConnect("oidc", options =>
        //    {
        //        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
        //        options.SignOutScheme = IdentityServerConstants.SignoutScheme;
        //        options.SaveTokens = true;

        //        options.Authority = "https://localhost:5001";
        //        options.ClientSecret = "secret";
        //        options.ResponseType = "code";
        //        options.Scope.Clear();
        //        options.Scope.Add("openid");
        //        options.Scope.Add("profile");
        //        options.Scope.Add("api1");
        //        //options.Scope.Add("offline_access");
        //        //options.Scope.Add("verification");
        //        options.ClaimActions.MapJsonKey("email_verified", "email_verified");
        //        options.GetClaimsFromUserInfoEndpoint = true;
        //        options.MapInboundClaims = false;

        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            NameClaimType = "name",
        //            RoleClaimType = "role",
        //        };
        //    });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        return builder.Build();
    }

    public static async Task<WebApplication> ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //InitializeDatabase(app); ????????


        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        //app.UseAuthentication();
        app.UseAuthorization();

        // uncomment if you want to add a UI
        //app.MapRazorPages().RequireAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<Data.DbContext>();
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await Initialize(context, userManager, roleManager);
        }

        return app;
    }


    public static async Task Initialize(Data.DbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        context.Database.EnsureCreated();

        if (!context.Users.Any())
        {
            // Create roles
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("User"));

            // Create a test user
            var user = new AppUser
            {
                Name = "Ali",
                UserName = "aliSmith@email.com",
                Email = "aliSmith@email.com",
                EmailConfirmed = true,
                NationalId = "6640009455",
                PhoneNumber = "09129513578",
                PhoneNumberConfirmed = true,
            };

            await userManager.CreateAsync(user, "Pass123$");
            await userManager.AddToRoleAsync(user, "User");

            // Create an admin user
            var adminUser = new AppUser
            {
                Name = "Salar",
                UserName = "salar",
                Email = "Salarghi@email.com",
                EmailConfirmed = true,
                NationalId = "6640009455",
                PhoneNumber = "09121597532",
                PhoneNumberConfirmed = true,
            };

            await userManager.CreateAsync(adminUser, "Admin123$");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Seed IdentityServer configuration data
        
        //using (var configContext = new ConfigurationDbContext(context.Database.GetDbConnection()))
        //{
        //    if (!configContext.Clients.Any())
        //    {
        //        foreach (var client in Config.Clients)
        //        {
        //            configContext.Clients.Add(client.ToEntity());
        //        }
        //        configContext.SaveChanges();
        //    }

        //    if (!configContext.IdentityResources.Any())
        //    {
        //        foreach (var resource in Config.IdentityResources)
        //        {
        //            configContext.IdentityResources.Add(resource.ToEntity());
        //        }
        //        configContext.SaveChanges();
        //    }

        //    if (!configContext.ApiScopes.Any())
        //    {
        //        foreach (var apiScope in Config.ApiScopes)
        //        {
        //            configContext.ApiScopes.Add(apiScope.ToEntity());
        //        }
        //        configContext.SaveChanges();
        //    }
        //}
    }
}
