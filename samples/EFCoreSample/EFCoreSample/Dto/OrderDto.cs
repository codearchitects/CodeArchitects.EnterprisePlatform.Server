using CodeArchitects.Platform.Data.Tracking;
using Newtonsoft.Json;

namespace EFCoreSample.Dto;

public class OrderDto : ITrackable
{
  public Guid Id { get; set; }
  public List<ProductDto>? Products { get; set; }

  [JsonProperty("$trackingState")]
  public TrackingState TrackingState { get; set; }
}
