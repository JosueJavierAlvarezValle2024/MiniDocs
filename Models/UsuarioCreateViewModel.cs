using System.ComponentModel.DataAnnotations;

namespace MiniDocs.Models;

public class UsuarioCreateViewModel
{
    [Required, StringLength(150)]
    [Display(Name = "Nombre completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Rol")]
    public string Rol { get; set; } = "Usuario";

    [Display(Name = "Departamento")]
    public int? DepartamentoId { get; set; }
}
