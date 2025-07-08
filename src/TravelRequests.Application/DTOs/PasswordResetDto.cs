using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs;

public class PasswordResetDto
{
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo es inválido")]
    public string email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener 6 caracteres")]
    public string code { get; set; } = string.Empty;

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 255 caracteres")]
    public string newPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare("newPassword", ErrorMessage = "Las contraseñas no coinciden")]
    public string confirmPassword { get; set; } = string.Empty;
}
