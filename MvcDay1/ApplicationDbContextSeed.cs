using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MvcDay1.Data;

using System.Linq;
using System.Threading.Tasks;

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

            var moderatorRole = new IdentityRole("Moderator");

            if (roleManager.Roles.All(r => r.Name != moderatorRole.Name))
            {
                await roleManager.CreateAsync(moderatorRole);
            }

            var userRole = new IdentityRole("User");

            if (roleManager.Roles.All(r => r.Name != userRole.Name))
            {
                await roleManager.CreateAsync(userRole);
            }

        }

        public static async Task SeedSampleDataAsync(ApplicationDbContext context)
        {
            if (!await context.Categories.AnyAsync())
            {
                await context.Categories.AddAsync(new Category() { Name = "Fiction" });
                await context.Categories.AddAsync(new Category() { Name = "Non-Fiction" });
                await context.Categories.AddAsync(new Category() { Name = "Documentary" });
                await context.Categories.AddAsync(new Category() { Name = "Computer Science" });
                await context.SaveChangesAsync();
            }

            if (!await context.Books.AnyAsync(b => b.Title == "ASP.NET Core"))
            {

                await context.Books.AddAsync(new Book()
                {
                    Title = "ASP.NET Core",
                    Price = 100m,
                    Category = await context.Categories.FirstAsync(c => c.Name == "Computer Science")
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
