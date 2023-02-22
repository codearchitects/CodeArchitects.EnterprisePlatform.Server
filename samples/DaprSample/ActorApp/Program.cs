using ActorApp.Domain;
using ActorApp.OldSkool;
using CodeArchitects.Platform.Actors.Dapr.AspNetCore;

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
    }));

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

app.MapActorsHandlers();

app.Run();
