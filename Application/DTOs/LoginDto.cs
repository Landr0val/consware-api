using System.ComponentModel.DataAnnotations;

namespace consware_api.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo es inválido")]
    public string email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    public string password { get; set; } = string.Empty;
}
