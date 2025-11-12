namespace BookstoreApplication.Models;
using Microsoft.AspNetCore.Identity;

public class SeedData
{
    public static async Task InitializeAsync(IServiceProvider sp)
    {
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();

        // Osiguraj da postoje role (za slucaj da pokreces bez migracija)
        if (!await roleManager.RoleExistsAsync("Bibliotekar"))
            await roleManager.CreateAsync(new IdentityRole("Bibliotekar"));
        if (!await roleManager.RoleExistsAsync("Urednik"))
            await roleManager.CreateAsync(new IdentityRole("Urednik"));

        // 3 urednika
        await EnsureEditorAsync(userManager, "john", "john@example.com", "John", "Doe");
        await EnsureEditorAsync(userManager, "jane", "jane@example.com", "Jane", "Doe");
        await EnsureEditorAsync(userManager, "nick", "nick@example.com", "Nick", "Smith");
    }

    private static async Task EnsureEditorAsync(
        UserManager<ApplicationUser> um, string username, string email, string name, string surname)
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
            // demo sifra sa velikim/malim/brojem/spec
            await um.CreateAsync(user, "Passw0rd!");
        }
        if (!await um.IsInRoleAsync(user, "Urednik"))
            await um.AddToRoleAsync(user, "Urednik");
    }
}
