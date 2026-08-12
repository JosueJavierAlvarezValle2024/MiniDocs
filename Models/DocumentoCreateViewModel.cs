using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MiniDocs.Models;

public class DocumentoCreateViewModel
{
    [Required, StringLength(200)]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }

    [Required]
    [Display(Name = "Departamento")]
    public int DepartamentoId { get; set; }

    [Required(ErrorMessage = "Selecciona un archivo.")]
    [Display(Name = "Archivo")]
    public IFormFile? Archivo { get; set; }
}
