using consware_api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace consware_api.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo es inválido")]
    [StringLength(255, ErrorMessage = "El correo no puede exceder 255 caracteres")]
    public string email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(255, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 255 caracteres")]
    public string password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "El rol especificado no es válido")]
    public UserRole role { get; set; }
}
