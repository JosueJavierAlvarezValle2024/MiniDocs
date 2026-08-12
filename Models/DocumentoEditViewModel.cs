using System.ComponentModel.DataAnnotations;

namespace MiniDocs.Models;

public class DocumentoEditViewModel
{
    public int Id { get; set; }
    [Required, StringLength(200)] public string Titulo { get; set; } = string.Empty;
    [StringLength(1000)] public string? Descripcion { get; set; }
    [Required] public int DepartamentoId { get; set; }
    public string NombreArchivoOriginal { get; set; } = string.Empty;
}
