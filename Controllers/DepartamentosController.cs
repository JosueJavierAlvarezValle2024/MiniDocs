using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Data;
using MiniDocs.Models;

namespace MiniDocs.Controllers;

[Authorize(Roles = "Administrador,SuperAdministrador")]
public class DepartamentosController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var departamentos = await context.Departamentos
            .OrderBy(d => d.Nombre)
            .ToListAsync();

        return View(departamentos);
    }

    public IActionResult Create() => View(new Departamento());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Departamento departamento)
    {
        if (await context.Departamentos.AnyAsync(d => d.Nombre == departamento.Nombre))
        {
            ModelState.AddModelError(nameof(departamento.Nombre), "Ya existe un departamento con ese nombre.");
        }

        if (!ModelState.IsValid)
        {
            return View(departamento);
        }

        departamento.FechaCreacion = DateTime.UtcNow;
        departamento.Activo = true;
        context.Departamentos.Add(departamento);
        await context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Departamento creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();

        var departamento = await context.Departamentos.FindAsync(id);
        return departamento is null ? NotFound() : View(departamento);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Departamento departamento)
    {
        if (id != departamento.Id) return NotFound();

        if (await context.Departamentos.AnyAsync(d => d.Id != id && d.Nombre == departamento.Nombre))
        {
            ModelState.AddModelError(nameof(departamento.Nombre), "Ya existe un departamento con ese nombre.");
        }

        if (!ModelState.IsValid) return View(departamento);

        var existente = await context.Departamentos.FindAsync(id);
        if (existente is null) return NotFound();

        existente.Nombre = departamento.Nombre;
        existente.Descripcion = departamento.Descripcion;
        existente.Activo = departamento.Activo;
        await context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Departamento actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var departamento = await context.Departamentos.FindAsync(id);
        if (departamento is null) return NotFound();

        departamento.Activo = !departamento.Activo;
        await context.SaveChangesAsync();

        TempData["SuccessMessage"] = departamento.Activo
            ? "Departamento activado correctamente."
            : "Departamento desactivado correctamente.";

        return RedirectToAction(nameof(Index));
    }
}
