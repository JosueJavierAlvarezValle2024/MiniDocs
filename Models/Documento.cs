using System.ComponentModel.DataAnnotations;

namespace MiniDocs.Models;

public class Documento
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required, StringLength(255)]
    public string NombreArchivoOriginal { get; set; } = string.Empty;

    [Required, StringLength(255)]
    public string NombreArchivoInterno { get; set; } = string.Empty;

    [Required, StringLength(500)]
    public string RutaArchivo { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string TipoMime { get; set; } = "application/octet-stream";

    public long TamanoBytes { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }
    public int DepartamentoId { get; set; }
    public Departamento? Departamento { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaActualizacion { get; set; }
}
