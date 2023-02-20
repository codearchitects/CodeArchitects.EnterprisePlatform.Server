using Dapr.Actors.Runtime;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace ActorApp.OldSkool;

public class TrafficLightActor : Actor, ITrafficLightActor, IRemindable
{
  private const string s_turnGreenReminder = "TurnGreen";
  private const string s_turnRedReminder = "TurnRed";
  private const string s_turnYellowReminder = "TurnYellow";

  public TrafficLightActor(ActorHost host)
    : base(host)
  {
  }

  public async Task StartAsync(CancellationToken cancellationToken = default)
  {
    TrafficLightState state = await GetStateAsync(cancellationToken);

    if (state.Color is not LightColor.Off)
      throw new InvalidOperationException("I was already started!");

    await TurnGreenAsync("traffic light is starting", 0);
    await StateManager.SaveStateAsync(cancellationToken);
  }

  public async Task<string> GetLightColorAsync(CancellationToken cancellationToken = default)
  {
    TrafficLightState state = await GetStateAsync(cancellationToken);

    return state.Color switch
    {
      LightColor.Red => "red",
      LightColor.Yellow => "yellow",
      LightColor.Green => "green",
      _ => throw new InvalidOperationException("I was not started!")
    };
  }

  public async Task StopAsync(CancellationToken cancellationToken = default)
  {
    TrafficLightState state = await GetStateAsync(cancellationToken);

    string reminderName = state.Color switch
    {
      LightColor.Red => s_turnGreenReminder,
      LightColor.Yellow => s_turnRedReminder,
      LightColor.Green => s_turnYellowReminder,
      _ => throw new InvalidOperationException("I am already off!")
    };

    await UnregisterReminderAsync(reminderName);

    Logger.LogInformation("Traffic light is stopping.");

    state.Color = LightColor.Off;
    await StateManager.SaveStateAsync(cancellationToken);
  }

  public async Task<TrafficLightResponse> CrossIntersectionAsync(CancellationToken cancellationToken = default)
  {
    TrafficLightState state = await GetStateAsync(cancellationToken);

    switch (state.Color)
    {
      case LightColor.Red:
        return new TrafficLightResponse
        {
          CanCross = false,
          TurnsGreenAt = state.TurnsGreenAt
        };
      case LightColor.Yellow:
        return new TrafficLightResponse
        {
          CanCross = true
        };
      case LightColor.Green:
        state.CarsBeforeYellow++;
        if (state.CarsBeforeYellow >= state.MaxCarsBeforeYellow)
        {
          state.MaxCarsBeforeYellow += 3;
          await UnregisterReminderAsync(s_turnYellowReminder);
          await TurnYellowAsync("too many cars have passed");
        }

        return new TrafficLightResponse
        {
          CanCross = true
        };
      default:
        throw new InvalidOperationException("I was not started!");
    }
  }

  public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
  {
    switch (reminderName)
    {
      case s_turnGreenReminder:
        TurnGreenReminderData data = JsonSerializer.Deserialize<TurnGreenReminderData>(state)!;
        return TurnGreenAsync(data.Reason, data.MaxCarsIncrement);
      case s_turnYellowReminder:
        return TurnYellowAsync(Encoding.UTF8.GetString(state));
      case s_turnRedReminder:
        return TurnRedAsync(Encoding.UTF8.GetString(state));
    }

    return Task.CompletedTask;
  }

  private async Task TurnGreenAsync(string reason, int maxCarsIncrement)
  {
    TrafficLightState state = await GetStateAsync();

    state.MaxCarsBeforeYellow = Math.Max(5, state.MaxCarsBeforeYellow + maxCarsIncrement);
    state.CarsBeforeYellow = 0;

    Logger.LogInformation("Turning green. Reason: {reason}. Max cars before yellow: {maxCars}.", reason, state.MaxCarsBeforeYellow);

    state.Color = LightColor.Green;

    await RegisterReminderAsync(s_turnYellowReminder, Encoding.UTF8.GetBytes("regular schedule"), TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan);

    await StateManager.SaveStateAsync();
  }

  private async Task TurnYellowAsync(string reason)
  {
    TrafficLightState state = await GetStateAsync();

    Logger.LogInformation("Turning yellow. Reason: {reason}.", reason);

    state.Color = LightColor.Yellow;

    await RegisterReminderAsync(s_turnRedReminder, Encoding.UTF8.GetBytes("regular schedule"), TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);

    await StateManager.SaveStateAsync();
  }

  private async Task TurnRedAsync(string reason)
  {
    TrafficLightState state = await GetStateAsync();

    Logger.LogInformation("Turning red. Reason: {reason}.", reason);

    state.Color = LightColor.Red;

    TimeSpan timer = TimeSpan.FromSeconds(30);
    state.TurnsGreenAt = DateTime.Now + timer;
    TurnGreenReminderData data = new()
    {
      Reason = "regular schedule",
      MaxCarsIncrement = -2
    };
    await RegisterReminderAsync(s_turnGreenReminder, JsonSerializer.SerializeToUtf8Bytes(data), timer, Timeout.InfiniteTimeSpan);

    await StateManager.SaveStateAsync();
  }

  private async Task<TrafficLightState> GetStateAsync(CancellationToken cancellationToken = default)
  {
    const string stateName = "state";

    var conditionalState = await StateManager.TryGetStateAsync<TrafficLightState>(stateName, cancellationToken);
    if (conditionalState.HasValue)
      return conditionalState.Value;

    var state = new TrafficLightState();
    await StateManager.AddStateAsync(stateName, state, cancellationToken);

    return state;
  }
}
