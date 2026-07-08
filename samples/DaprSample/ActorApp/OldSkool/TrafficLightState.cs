namespace ActorApp.OldSkool;

public class TrafficLightState
{
  public LightColor Color { get; set; }
  public int MaxCarsBeforeYellow { get; set; } = 5;
  public int CarsBeforeYellow { get; set; }
  public DateTime TurnsGreenAt { get; set; }
}
