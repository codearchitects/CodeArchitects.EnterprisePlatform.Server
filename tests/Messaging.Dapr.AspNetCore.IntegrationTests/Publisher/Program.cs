using CodeArchitects.Platform.Messaging;
using Microsoft.AspNetCore.Mvc;
using Publisher;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddDaprInfrastructure(opt => opt
    .SetConfiguration(builder.Configuration))
  .AddMessaging();

var app = builder.Build();

app.MapGet("/noresult/{topic}/send/{id}", async (string topic, Guid id, [FromServices] IMessageBus bus, CancellationToken cancellationToken) =>
{
  await bus.SendAsync(topic, new NoResultMessage(id), cancellationToken);
  return Results.Ok();
});

app.MapGet("/withresult/{topic}/send/{id}", async (string topic, Guid id, [FromServices] IMessageBus bus, CancellationToken cancellationToken) =>
{
  await bus.SendAsync(topic, new WithResultMessage(id), cancellationToken);
  return Results.Ok();
});

#if NET9_0
Console.WriteLine("Executing app with net9 runtime...");
#elif NET8_0
Console.WriteLine("Executing app with net8 runtime...");
#elif NET7_0
Console.WriteLine("Executing app with net7 runtime...");
#endif

app.Run();
