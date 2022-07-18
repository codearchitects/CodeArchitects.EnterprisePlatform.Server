using CodeArchitects.Platform.Messaging;
using Microsoft.AspNetCore.Mvc;
using Publisher.NoResponse;

var builder = WebApplication.CreateBuilder(args);

builder.Services
  .AddDaprInfrastructure(opt => opt
    .SetConfiguration(builder.Configuration))
  .AddMessaging();

var app = builder.Build();

app.MapGet("/noresponse/{topic}/send/{id}", async (string topic, Guid id, [FromServices] IMessageBus bus, CancellationToken cancellationToken) =>
{
  await bus.SendAsync(topic, new NoResponseMessage(id), cancellationToken);
  return Results.Ok();
});

app.Run();
