using System.ComponentModel.DataAnnotations;

namespace MVC_app_001.Models;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Contraseña { get; set; } = string.Empty;

    public bool Recordarme { get; set; }
}
