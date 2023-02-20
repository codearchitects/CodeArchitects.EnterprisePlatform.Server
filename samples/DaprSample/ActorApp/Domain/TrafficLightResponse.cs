namespace ActorApp.Domain;

public class TrafficLightResponse
{
  public bool CanCross { get; set; }
  public DateTime TurnsGreenAt { get; set; }
}
