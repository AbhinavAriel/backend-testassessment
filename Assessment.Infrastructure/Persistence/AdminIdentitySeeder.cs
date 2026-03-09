using Assessment.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assessment.Infrastructure.Persistence
{
    public static class AdminIdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            const string adminRole = "Admin";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
                if (!roleResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(" | ", roleResult.Errors.Select(x => x.Description)));
                }
            }

            var adminSection = configuration.GetSection("AdminUser");
            var fullName = adminSection["FullName"] ?? "Ariel Admin";
            var email = adminSection["Email"] ?? "admin@arielsoftwares.in";
            var password = adminSection["Password"] ?? "ArielAdmin@123";

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    FullName = fullName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(" | ", createResult.Errors.Select(x => x.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(user, adminRole))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, adminRole);
                if (!addRoleResult.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(" | ", addRoleResult.Errors.Select(x => x.Description)));
                }
            }
        }
    }
}