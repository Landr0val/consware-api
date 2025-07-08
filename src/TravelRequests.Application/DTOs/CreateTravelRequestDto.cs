using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs;

public class CreateTravelRequestDto
{
    [Required(ErrorMessage = "La ciudad de origen es requerida")]
    [StringLength(100, ErrorMessage = "La ciudad de origen no puede exceder 100 caracteres")]
    public string origin_city { get; set; } = string.Empty;

    [Required(ErrorMessage = "La ciudad de destino es requerida")]
    [StringLength(100, ErrorMessage = "La ciudad de destino no puede exceder 100 caracteres")]
    public string destination_city { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de ida es requerida")]
    public DateTime departure_date { get; set; }

    [Required(ErrorMessage = "La fecha de regreso es requerida")]
    public DateTime return_date { get; set; }

    [Required(ErrorMessage = "La justificación es requerida")]
    [StringLength(500, ErrorMessage = "La justificación no puede exceder 500 caracteres")]
    public string justification { get; set; } = string.Empty;
}
