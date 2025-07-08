namespace TravelRequests.Domain.Entities;

public class PasswordResetCode
{
    public int id { get; set; }
    public int user_id { get; set; }
    public string code { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public DateTime created_at { get; set; }
    public DateTime expires_at { get; set; }
    public bool used { get; set; } = false;
    public DateTime? used_at { get; set; }

    public User user { get; set; } = null!;
}
