using System.ComponentModel.DataAnnotations;

namespace MiniDocs.Models;

public class UsuarioEditViewModel
{
    public string Id { get; set; } = string.Empty;
    [Required, StringLength(150)] public string NombreCompleto { get; set; } = string.Empty;
    [Required, EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Rol { get; set; } = "Usuario";
    public int? DepartamentoId { get; set; }
    public bool Activo { get; set; }
}
