using System.ComponentModel.DataAnnotations;

namespace consware_api.Application.DTOs;

public class PasswordResetRequestDto
{
    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo es inválido")]
    public string email { get; set; } = string.Empty;
}
