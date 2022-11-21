using CodeArchitects.Platform.Data.Tracking;
using Newtonsoft.Json;

namespace EFCoreSample.Dto;

public class ProductDto : ITrackable
{
  public Guid Id { get; set; }
  public string? Name { get; set; }

  [JsonProperty("$trackingState")]
  public TrackingState TrackingState { get; set; }
}
