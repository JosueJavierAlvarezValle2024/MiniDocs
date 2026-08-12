using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Models;

namespace MiniDocs.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var context = services.GetRequiredService<ApplicationDbContext>();

        foreach (var role in new[] { "SuperAdministrador", "Administrador", "Usuario" })
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

        foreach (var nombre in new[] { "Finanzas", "Tecnología", "Operaciones", "Dirección", "Jurídico", "Ventas" })
        {
            if (!await context.Departamentos.AnyAsync(d => d.Nombre == nombre))
                context.Departamentos.Add(new Departamento { Nombre = nombre, Activo = true });
        }
        await context.SaveChangesAsync();

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
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                NombreCompleto = "Administrador de MiniDocs"
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

        var superEmail = configuration["SuperAdminSeed:Email"];
        var superPassword = configuration["SuperAdminSeed:Password"];
        if (string.IsNullOrWhiteSpace(superEmail) || string.IsNullOrWhiteSpace(superPassword))
        {
            logger.LogWarning("No se creó el SuperAdministrador inicial. Configura SuperAdminSeed:Email y SuperAdminSeed:Password.");
            return;
        }

        var superAdmin = await userManager.FindByEmailAsync(superEmail);
        if (superAdmin is null)
        {
            superAdmin = new ApplicationUser
            {
                UserName = superEmail,
                Email = superEmail,
                EmailConfirmed = true,
                NombreCompleto = "Superadministrador de MiniDocs",
                Activo = true
            };
            var result = await userManager.CreateAsync(superAdmin, superPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException($"No se pudo crear el SuperAdministrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        if (!await userManager.IsInRoleAsync(superAdmin, "SuperAdministrador"))
        {
            var result = await userManager.AddToRoleAsync(superAdmin, "SuperAdministrador");
            if (!result.Succeeded)
                throw new InvalidOperationException($"No se pudo asignar el rol SuperAdministrador: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }
}
