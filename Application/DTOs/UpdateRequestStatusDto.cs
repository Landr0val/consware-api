using consware_api.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace consware_api.Application.DTOs;

public class UpdateRequestStatusDto
{
    [Required(ErrorMessage = "El estado es requerido")]
    [EnumDataType(typeof(RequestStatus), ErrorMessage = "El estado especificado no es válido")]
    public RequestStatus status { get; set; }
}
