using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Multiplify.Application.Constants;
using Multiplify.Domain;

namespace Multiplify.Infrastructure.SeedData;
public class ApplicationSeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;
        var context = provider.GetRequiredService<ApplicationDbContext>();

        //Add default roles
        List<IdentityRole> defaultRoles =
        [
            new IdentityRole { Name = ApplicationRoles.SuperAdmin},
            new IdentityRole { Name = ApplicationRoles.Member},
        ];

        using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
        {
            foreach (var role in defaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!))
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }


        using var userManager = provider.GetRequiredService<UserManager<AppUser>>();
        if (!context.Users.Any())
        {
            var superAdmin = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "Super",
                LastName = "Admin",
                UserName = "superadmin",
                Email = "superadmin@app.com",
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(superAdmin, "SuperAdmin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(superAdmin, ApplicationRoles.SuperAdmin);
        }
    }
}
