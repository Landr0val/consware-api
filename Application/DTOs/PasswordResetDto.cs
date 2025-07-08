using System.ComponentModel.DataAnnotations;

namespace consware_api.Application.DTOs;

public class PasswordResetDto
{
    [Required(ErrorMessage = "El token es requerido")]
    public string token { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 255 caracteres")]
    public string newPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare("newPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string confirmPassword { get; set; } = string.Empty;
}
