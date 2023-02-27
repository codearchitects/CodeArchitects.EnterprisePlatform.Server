namespace ActorApp.Domain;

public class TrafficLightState
{
  public int MaxCarsBeforeYellow { get; set; } = 5;
  public int CarsBeforeYellow { get; set; }
  public DateTime TurnsGreenAt { get; set; }
}
