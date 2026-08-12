using Microsoft.AspNetCore.Http;
using MiniDocs.Services;

namespace MiniDocs.Tests;

public class DocumentoArchivoValidatorTests
{
    [Fact]
    public void Acepta_extension_permitida()
    {
        var archivo = CrearArchivo("manual.PDF", 128);
        Assert.Empty(DocumentoArchivoValidator.Validar(archivo));
    }

    [Fact]
    public void Rechaza_extension_no_permitida()
    {
        var archivo = CrearArchivo("script.exe", 128);
        Assert.Contains(DocumentoArchivoValidator.Validar(archivo), e => e.Contains("no está permitido"));
    }

    [Fact]
    public void Rechaza_archivo_mayor_a_10_megabytes()
    {
        var archivo = CrearArchivo("grande.pdf", DocumentoArchivoValidator.TamanoMaximo + 1);
        Assert.Contains(DocumentoArchivoValidator.Validar(archivo), e => e.Contains("10 MB"));
    }

    [Fact]
    public void Requiere_archivo()
    {
        Assert.Contains("Selecciona un archivo.", DocumentoArchivoValidator.Validar(null));
    }

    private static IFormFile CrearArchivo(string nombre, long longitud)
    {
        var stream = new MemoryStream(new byte[(int)Math.Min(longitud, 1024)]);
        return new FormFile(stream, 0, longitud, "Archivo", nombre);
    }
}
