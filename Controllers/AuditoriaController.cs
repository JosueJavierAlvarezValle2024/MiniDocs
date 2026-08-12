using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Data;

namespace MiniDocs.Controllers;

[Authorize(Roles = "Administrador,SuperAdministrador")]
public class AuditoriaController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var registros = await context.AuditoriasDocumentos
            .Include(a => a.Documento)
            .Include(a => a.Usuario)
            .OrderByDescending(a => a.Fecha)
            .Take(200)
            .ToListAsync();
        return View(registros);
    }
}
