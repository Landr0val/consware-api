using TravelRequests.Domain.Enums;

namespace TravelRequests.Domain.Entities;

public class User
{
    public int id { get; set; }
    public string name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public UserRole role { get; set; }
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
    public bool active { get; set; } = true;
    public ICollection<TravelRequest> travel_requests { get; set; } = new List<TravelRequest>();
}
