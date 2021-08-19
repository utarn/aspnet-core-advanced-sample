using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using WebAPIDay2.Data;

namespace MvcDay1
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var administratorRole = new IdentityRole("Administrator");

            if (roleManager.Roles.All(r => r.Name != administratorRole.Name))
            {
                await roleManager.CreateAsync(administratorRole);
            }

            var userRole = new IdentityRole("User");

            if (roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await roleManager.CreateAsync(userRole);
            }

            var adminUser = await userManager.FindByEmailAsync("admin@localhost");
            if (adminUser == null)
            {
                var newUser = new ApplicationUser()
                {
                    Email = "admin@localhost",
                    UserName = "admin@localhost",
                    ApplicationName = "App 1",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(newUser, "Admin123!!");

                await userManager.AddToRoleAsync(newUser, "Administrator");
            }
        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
           // TODO

        }
    }
}
