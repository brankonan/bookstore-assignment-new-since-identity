using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BookstoreApplication.Models
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync("Bibliotekar"))
                await roleManager.CreateAsync(new IdentityRole("Bibliotekar"));

            if (!await roleManager.RoleExistsAsync("Urednik"))
                await roleManager.CreateAsync(new IdentityRole("Urednik"));

            await EnsureEditorAsync(userManager, "john", "john@example.com", "John", "Doe");
            await EnsureEditorAsync(userManager, "jane", "jane@example.com", "Jane", "Doe");
            await EnsureEditorAsync(userManager, "nick", "nick@example.com", "Nick", "Smith");

            var brankonan = await userManager.FindByNameAsync("brankonan");
            if (brankonan != null)
            {
                if (!await userManager.IsInRoleAsync(brankonan, "Urednik"))
                {
                    await userManager.AddToRoleAsync(brankonan, "Urednik");
                }
            }
        }

        private static async Task EnsureEditorAsync(
            UserManager<ApplicationUser> um,
            string username,
            string email,
            string name,
            string surname)
        {
            var user = await um.FindByNameAsync(username);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    Name = name,
                    Surname = surname,
                    EmailConfirmed = true
                };
                await um.CreateAsync(user, "Passw0rd!");
            }

            if (!await um.IsInRoleAsync(user, "Urednik"))
            {
                await um.AddToRoleAsync(user, "Urednik");
            }
        }
    }
}
