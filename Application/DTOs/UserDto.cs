using consware_api.Domain.Enums;

namespace consware_api.Application.DTOs;

public class UserDto
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public UserRole role { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public bool active { get; set; }
}
