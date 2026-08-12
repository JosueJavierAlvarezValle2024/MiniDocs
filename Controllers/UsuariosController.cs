using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Data;
using MiniDocs.Models;

namespace MiniDocs.Controllers;

[Authorize(Roles = "Administrador")]
public class UsuariosController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var usuarios = await context.Users
            .Include(u => u.Departamento)
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();

        var roles = new Dictionary<string, string>();
        foreach (var usuario in usuarios)
        {
            var role = (await userManager.GetRolesAsync(usuario)).FirstOrDefault() ?? "Sin rol";
            roles[usuario.Id] = role;
        }

        ViewBag.Roles = roles;
        return View(usuarios);
    }

    public async Task<IActionResult> Create()
    {
        await CargarOpcionesAsync();
        return View(new UsuarioCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync();
            return View(model);
        }

        var usuario = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            NombreCompleto = model.NombreCompleto,
            DepartamentoId = model.DepartamentoId,
            EmailConfirmed = true,
            Activo = true
        };

        var result = await userManager.CreateAsync(usuario, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            await CargarOpcionesAsync();
            return View(model);
        }

        await userManager.AddToRoleAsync(usuario, model.Rol);
        TempData["SuccessMessage"] = "Usuario creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string id)
    {
        var usuario = await userManager.FindByIdAsync(id);
        if (usuario is null) return NotFound();

        usuario.Activo = !usuario.Activo;
        await userManager.UpdateAsync(usuario);

        TempData["SuccessMessage"] = usuario.Activo
            ? "Usuario activado correctamente."
            : "Usuario desactivado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    private async Task CargarOpcionesAsync()
    {
        ViewBag.Departamentos = new SelectList(
            await context.Departamentos.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(),
            nameof(Departamento.Id), nameof(Departamento.Nombre));

        ViewBag.Roles = new SelectList(
            await roleManager.Roles.OrderBy(r => r.Name).ToListAsync(),
            nameof(IdentityRole.Name), nameof(IdentityRole.Name));
    }
}
