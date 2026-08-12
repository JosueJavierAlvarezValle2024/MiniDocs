using System.ComponentModel.DataAnnotations;

namespace MiniDocs.Models;

public class AuditoriaDocumento
{
    public int Id { get; set; }
    public int DocumentoId { get; set; }
    public Documento? Documento { get; set; }
    [Required, StringLength(50)] public string Accion { get; set; } = string.Empty;
    [Required] public string UsuarioId { get; set; } = string.Empty;
    public ApplicationUser? Usuario { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
