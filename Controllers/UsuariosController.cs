using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Data;
using MiniDocs.Models;

namespace MiniDocs.Controllers;

[Authorize(Roles = "Administrador,SuperAdministrador")]
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

    public async Task<IActionResult> Edit(string id)
    {
        var usuario = await userManager.FindByIdAsync(id);
        if (usuario is null) return NotFound();
        await CargarOpcionesAsync();
        return View(new UsuarioEditViewModel
        {
            Id = usuario.Id,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email ?? string.Empty,
            DepartamentoId = usuario.DepartamentoId,
            Activo = usuario.Activo,
            Rol = (await userManager.GetRolesAsync(usuario)).FirstOrDefault() ?? "Usuario"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioEditViewModel model)
    {
        var usuario = await userManager.FindByIdAsync(model.Id);
        if (usuario is null) return NotFound();
        if (!ModelState.IsValid)
        {
            await CargarOpcionesAsync();
            return View(model);
        }

        usuario.NombreCompleto = model.NombreCompleto;
        usuario.DepartamentoId = model.DepartamentoId;
        usuario.Activo = model.Activo;
        var emailResult = await userManager.SetEmailAsync(usuario, model.Email);
        var nameResult = await userManager.SetUserNameAsync(usuario, model.Email);
        var updateResult = await userManager.UpdateAsync(usuario);
        if (!emailResult.Succeeded || !nameResult.Succeeded || !updateResult.Succeeded)
        {
            foreach (var error in emailResult.Errors.Concat(nameResult.Errors).Concat(updateResult.Errors))
                ModelState.AddModelError(string.Empty, error.Description);
            await CargarOpcionesAsync();
            return View(model);
        }

        var rolesActuales = await userManager.GetRolesAsync(usuario);
        await userManager.RemoveFromRolesAsync(usuario, rolesActuales);
        await userManager.AddToRoleAsync(usuario, model.Rol);
        TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
        return RedirectToAction(nameof(Index));
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

        var nombresPermitidos = User.IsInRole("SuperAdministrador")
            ? new[] { "Administrador", "Usuario", "SuperAdministrador" }
            : new[] { "Usuario" };
        ViewBag.Roles = new SelectList(nombresPermitidos);
    }
}
