using CodeArchitects.Platform.Data.Tracking;
using Newtonsoft.Json;

namespace EFCoreSample.Dto;

public class CartDto : ITrackable
{
  public Guid Id { get; set; }
  public List<OrderDto>? Orders { get; set; }

  [JsonProperty("$trackingState")]
  public TrackingState TrackingState { get; set; }
}
