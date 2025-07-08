namespace consware_api.Application.DTOs;

public class TokenResponseDto
{
    public string token { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public DateTime expires_at { get; set; }
    public string email { get; set; } = string.Empty;
}
