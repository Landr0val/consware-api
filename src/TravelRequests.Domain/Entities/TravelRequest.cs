using TravelRequests.Domain.Enums;

namespace TravelRequests.Domain.Entities;

public class TravelRequest
{
    public int id { get; set; }
    public int user_id { get; set; }
    public User user { get; set; } = null!;
    public string origin_city { get; set; } = string.Empty;
    public string destination_city { get; set; } = string.Empty;
    public DateTime departure_date { get; set; }
    public DateTime return_date { get; set; }
    public string justification { get; set; } = string.Empty;
    public RequestStatus status { get; set; } = RequestStatus.Pending;
    public DateTime created_at { get; set; }
    public DateTime? updated_at { get; set; }
}
