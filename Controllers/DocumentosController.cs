using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Data;
using MiniDocs.Models;

namespace MiniDocs.Controllers;

[Authorize]
public class DocumentosController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    IWebHostEnvironment environment) : Controller
{
    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".jpg", ".jpeg", ".png" };

    private const long TamanoMaximo = 10 * 1024 * 1024;

    public async Task<IActionResult> Index(string? busqueda, int? departamentoId)
    {
        var esAdmin = User.IsInRole("Administrador");
        var consulta = context.Documentos
            .Include(d => d.Departamento)
            .Include(d => d.Usuario)
            .Where(d => d.Activo);

        if (!esAdmin)
        {
            var usuarioId = userManager.GetUserId(User);
            consulta = consulta.Where(d => d.UsuarioId == usuarioId);
        }

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            consulta = consulta.Where(d => d.Titulo.Contains(busqueda));
        }

        if (departamentoId.HasValue)
        {
            consulta = consulta.Where(d => d.DepartamentoId == departamentoId.Value);
        }

        ViewBag.Busqueda = busqueda;
        ViewBag.DepartamentoId = departamentoId;
        ViewBag.Departamentos = new SelectList(
            await context.Departamentos.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(),
            nameof(Departamento.Id), nameof(Departamento.Nombre), departamentoId);

        return View(await consulta.OrderByDescending(d => d.FechaCreacion).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await CargarDepartamentosAsync();
        return View(new DocumentoCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DocumentoCreateViewModel model)
    {
        if (model.Archivo is not null)
        {
            var extension = Path.GetExtension(model.Archivo.FileName);
            if (!ExtensionesPermitidas.Contains(extension))
                ModelState.AddModelError(nameof(model.Archivo), "El tipo de archivo no está permitido.");
            if (model.Archivo.Length > TamanoMaximo)
                ModelState.AddModelError(nameof(model.Archivo), "El archivo no puede superar 10 MB.");
        }

        var departamentoValido = await context.Departamentos.AnyAsync(d => d.Id == model.DepartamentoId && d.Activo);
        if (!departamentoValido)
            ModelState.AddModelError(nameof(model.DepartamentoId), "Selecciona un departamento activo.");

        if (!ModelState.IsValid)
        {
            await CargarDepartamentosAsync();
            return View(model);
        }

        var carpeta = Path.Combine(environment.ContentRootPath, "App_Data", "Uploads");
        Directory.CreateDirectory(carpeta);
        var nombreInterno = $"{Guid.NewGuid():N}{Path.GetExtension(model.Archivo!.FileName)}";
        var rutaCompleta = Path.Combine(carpeta, nombreInterno);
        await using (var stream = System.IO.File.Create(rutaCompleta))
        {
            await model.Archivo.CopyToAsync(stream);
        }

        var documento = new Documento
        {
            Titulo = model.Titulo,
            Descripcion = model.Descripcion,
            NombreArchivoOriginal = Path.GetFileName(model.Archivo.FileName),
            NombreArchivoInterno = nombreInterno,
            RutaArchivo = Path.Combine("App_Data", "Uploads", nombreInterno),
            TipoMime = model.Archivo.ContentType,
            TamanoBytes = model.Archivo.Length,
            UsuarioId = userManager.GetUserId(User)!,
            DepartamentoId = model.DepartamentoId
        };

        context.Documentos.Add(documento);
        await context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Documento guardado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Download(int id)
    {
        var documento = await ObtenerPermitidoAsync(id);
        if (documento is null) return NotFound();
        var ruta = Path.Combine(environment.ContentRootPath, documento.RutaArchivo);
        if (!System.IO.File.Exists(ruta)) return NotFound("El archivo no existe en el almacenamiento.");
        return PhysicalFile(ruta, documento.TipoMime, documento.NombreArchivoOriginal);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var documento = await ObtenerPermitidoAsync(id);
        if (documento is null) return NotFound();
        documento.Activo = false;
        documento.FechaActualizacion = DateTime.UtcNow;
        await context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Documento eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<Documento?> ObtenerPermitidoAsync(int id)
    {
        var documento = await context.Documentos.FirstOrDefaultAsync(d => d.Id == id && d.Activo);
        if (documento is null || User.IsInRole("Administrador")) return documento;
        return documento.UsuarioId == userManager.GetUserId(User) ? documento : null;
    }

    private async Task CargarDepartamentosAsync()
    {
        ViewBag.Departamentos = new SelectList(
            await context.Departamentos.Where(d => d.Activo).OrderBy(d => d.Nombre).ToListAsync(),
            nameof(Departamento.Id), nameof(Departamento.Nombre));
    }
}
