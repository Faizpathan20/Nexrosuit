using Microsoft.AspNetCore.Identity;

namespace NexoraSuite.Data
{
    public static class IdentitySeeder
    {
        public const string AdminRole = "Admin";

        public static async Task SeedAdminRoleAsync(this IServiceProvider services, string? adminEmail)
        {
            using var scope = services.CreateScope();
            var provider = scope.ServiceProvider;

            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = provider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IdentitySeeder));

            if (!await roleManager.RoleExistsAsync(AdminRole))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(AdminRole));
                if (!roleResult.Succeeded)
                {
                    logger.LogError("Could not create {Role} role: {Errors}",
                        AdminRole, string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                    return;
                }
                logger.LogInformation("Created {Role} role.", AdminRole);
            }

            if (string.IsNullOrWhiteSpace(adminEmail))
            {
                logger.LogWarning(
                    "No AdminEmail configured under \"AdminSettings\". The {Role} role exists but nobody is assigned to it.",
                    AdminRole);
                return;
            }

            var user = await userManager.FindByEmailAsync(adminEmail.Trim());
            if (user == null)
            {
                logger.LogWarning("Admin user {Email} not found. Register that email first, then restart the app.", adminEmail);
                return;
            }

            if (await userManager.IsInRoleAsync(user, AdminRole))
            {
                return;
            }

            var assignResult = await userManager.AddToRoleAsync(user, AdminRole);
            if (assignResult.Succeeded)
            {
                logger.LogInformation("Assigned {Role} role to {Email}.", AdminRole, adminEmail);
            }
            else
            {
                logger.LogError("Could not assign {Role} to {Email}: {Errors}",
                    AdminRole, adminEmail, string.Join("; ", assignResult.Errors.Select(e => e.Description)));
            }
        }
    }
}