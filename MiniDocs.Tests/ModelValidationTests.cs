using System.ComponentModel.DataAnnotations;
using MiniDocs.Models;

namespace MiniDocs.Tests;

public class ModelValidationTests
{
    [Fact]
    public void Departamento_sin_nombre_es_invalido()
    {
        var model = new Departamento();
        var resultados = Validar(model);
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Departamento.Nombre)));
    }

    [Fact]
    public void Documento_requiere_titulo()
    {
        var model = new Documento { NombreArchivoOriginal = "manual.pdf", NombreArchivoInterno = "abc.pdf", RutaArchivo = "App_Data/Uploads/abc.pdf", TipoMime = "application/pdf", UsuarioId = "usuario", DepartamentoId = 1 };
        var resultados = Validar(model);
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(Documento.Titulo)));
    }

    [Fact]
    public void Usuario_requiere_correo_y_contrasena()
    {
        var model = new UsuarioCreateViewModel { NombreCompleto = "Usuario de prueba" };
        var resultados = Validar(model);
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(UsuarioCreateViewModel.Email)));
        Assert.Contains(resultados, r => r.MemberNames.Contains(nameof(UsuarioCreateViewModel.Password)));
    }

    [Fact]
    public void Usuario_con_datos_validos_pasa_validacion()
    {
        var model = new UsuarioCreateViewModel { NombreCompleto = "Usuario de prueba", Email = "usuario@minidocs.local", Password = "Prueba2026!", Rol = "Usuario" };
        Assert.Empty(Validar(model));
    }

    private static List<ValidationResult> Validar(object model)
    {
        var contexto = new ValidationContext(model);
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(model, contexto, resultados, validateAllProperties: true);
        return resultados;
    }
}
