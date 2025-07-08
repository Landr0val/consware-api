using System.ComponentModel.DataAnnotations;

namespace TravelRequests.Application.DTOs;

public class PasswordResetRequestResponseDto
{
    public string message { get; set; } = string.Empty;
    public string code { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public DateTime expires_at { get; set; }
}
