using ActorApp.Domain;
using ActorApp.OldSkool;
using CodeArchitects.Platform.Actors.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDaprInfrastructure(options => options
  .SetConfiguration(builder.Configuration))
  .AddActors(actors => actors
    .AddActor<TrafficLight>()
    .AccessPrivates()
    .ConfigureRuntimeOptions(runtimeOptions =>
    {
      runtimeOptions.Actors.RegisterActor<TrafficLightActor>();
    }))
  .AddMessaging(messaging => messaging
    .AddMessage(typeof(TurnOffCommand))
    .ScanAssembly(ActorMessaging.Assembly));

builder.Services
  .AddEndpointsApiExplorer()
  .AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();

app.MapMessageHandlers();

app.MapActorsHandlers();

app.Run();
