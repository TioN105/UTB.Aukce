using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UTB.Aukce.Domain.Entities;

namespace UTB.Aukce.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. Vytvoření rolí podle zadání
            string[] roles = { "Admin", "Manager", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Výchozí Admin účet
            await CreateUserIfNotExistsAsync(userManager, "admin@utb.cz", "Admin123", "Hlavní", "Administrátor", "Admin");

            // 3. Výchozí Manager účet
            await CreateUserIfNotExistsAsync(userManager, "manager@utb.cz", "Manager123", "Jan", "Manažer", "Manager");

            // 4. Výchozí Zákazník účet
            await CreateUserIfNotExistsAsync(userManager, "zakaznik@utb.cz", "Zakaznik123", "Petr", "Novák", "Customer");

            // 5. Základní kategorie aukcí
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "Elektronika", Description = "Mobily, notebooky, audio a herní konzole" },
                    new Category { Name = "Auto-moto", Description = "Osobní automobily, motocykly a náhradní díly" },
                    new Category { Name = "Starožitnosti a umění", Description = "Obrazy, sběratelské předměty a historické kousky" },
                    new Category { Name = "Sport a volný čas", Description = "Kola, fitness vybavení a outdoor" }
                );
                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateUserIfNotExistsAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}