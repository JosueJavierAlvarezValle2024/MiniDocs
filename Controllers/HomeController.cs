using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniDocs.Data;
using MiniDocs.Models;

namespace MiniDocs.Controllers;

public class HomeController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var dashboard = new DashboardViewModel
        {
            EsAdministrador = User.IsInRole("Administrador") || User.IsInRole("SuperAdministrador")
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            dashboard.Documentos = await context.Documentos.CountAsync(d => d.Activo);
            dashboard.Departamentos = await context.Departamentos.CountAsync(d => d.Activo);
            if (dashboard.EsAdministrador)
                dashboard.Usuarios = await context.Users.CountAsync(u => u.Activo);
        }

        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
