using Microsoft.AspNetCore.Identity;

namespace MiniDocs.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        foreach (var role in new[] { "Administrador", "Usuario" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"No se pudo crear el rol '{role}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }

        var adminEmail = configuration["AdminSeed:Email"];
        var adminPassword = configuration["AdminSeed:Password"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("No se creó el administrador inicial. Configura AdminSeed:Email y AdminSeed:Password mediante User Secrets o variables de entorno.");
            return;
        }

        var admin = await userManager.FindByEmailAsync(adminEmail);
        if (admin is null)
        {
            admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo crear el administrador inicial: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(admin, "Administrador"))
        {
            var result = await userManager.AddToRoleAsync(admin, "Administrador");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo asignar el rol Administrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
