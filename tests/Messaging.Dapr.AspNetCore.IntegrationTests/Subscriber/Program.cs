using CodeArchitects.Platform.Messaging.Bindings;
using Microsoft.AspNetCore.Mvc;
using Subscriber;
using Subscriber.WithResult;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddDaprInfrastructure(opt => opt
    .SetConfiguration(builder.Configuration))
  .AddMessaging();

builder.Services.AddSingleton<MessageAwaiter>();
builder.Services.AddSingleton<IOutputBinding<ITestOutputMetadata>, TestOutputBinding>();

var app = builder.Build();

app.MapGet("/wait/{id}", async (Guid id, [FromQuery] int? millisecondsTimeout, [FromServices] MessageAwaiter awaiter, CancellationToken cancellationToken) =>
{
  Task task = awaiter.GetTask(id);
  Task timer = Task.Delay(millisecondsTimeout ?? 10000, cancellationToken);
  Task winner = await Task.WhenAny(task, timer);
  await winner;

  return winner == task ? Results.Ok() : Results.NoContent();
});

app.MapMessageHandlers();

app.Run();
