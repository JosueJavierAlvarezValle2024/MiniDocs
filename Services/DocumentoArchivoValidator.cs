using Microsoft.AspNetCore.Http;

namespace MiniDocs.Services;

public static class DocumentoArchivoValidator
{
    public const long TamanoMaximo = 10 * 1024 * 1024;

    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".jpg", ".jpeg", ".png" };

    public static IReadOnlyList<string> Validar(IFormFile? archivo)
    {
        var errores = new List<string>();
        if (archivo is null)
        {
            errores.Add("Selecciona un archivo.");
            return errores;
        }

        if (!ExtensionesPermitidas.Contains(Path.GetExtension(archivo.FileName)))
            errores.Add("El tipo de archivo no está permitido.");
        if (archivo.Length > TamanoMaximo)
            errores.Add("El archivo no puede superar 10 MB.");

        return errores;
    }
}
