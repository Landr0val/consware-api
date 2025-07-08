using TravelRequests.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs;

public class UpdateUserDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "El formato del correo es inválido")]
    [StringLength(255, ErrorMessage = "El correo no puede exceder 255 caracteres")]
    public string email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El rol es requerido")]
    [EnumDataType(typeof(UserRole), ErrorMessage = "El rol especificado no es válido")]
    public UserRole role { get; set; }

    public bool active { get; set; }
}
