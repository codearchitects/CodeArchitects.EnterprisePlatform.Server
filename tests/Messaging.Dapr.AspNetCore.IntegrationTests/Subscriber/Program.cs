using Microsoft.AspNetCore.Mvc;
using Subscriber.NoResponse;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddDaprInfrastructure(opt => opt
    .SetConfiguration(builder.Configuration))
  .AddMessaging();

builder.Services.AddSingleton<NoResponseAwaiter>();

var app = builder.Build();

app.MapGet("/noresponse/wait/{id}", async (Guid id, [FromQuery] int? millisecondsTimeout, [FromServices] NoResponseAwaiter awaiter, CancellationToken cancellationToken) =>
{
  Task task = awaiter.GetTask(id);
  Task timer = Task.Delay(millisecondsTimeout ?? 10000, cancellationToken);
  Task winner = await Task.WhenAny(task, timer);
  await winner;

  return winner == task ? Results.Ok() : Results.NoContent();
});

app.MapMessageHandlers();

app.Run();
