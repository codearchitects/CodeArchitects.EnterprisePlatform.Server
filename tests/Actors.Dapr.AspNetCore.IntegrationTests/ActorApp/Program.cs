using ActorApp.Domain;
using CodeArchitects.Platform.Actors.Dapr.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDaprInfrastructure(options => options.SetConfiguration(builder.Configuration))
  .AddActors(actors => actors
    .AccessPrivates()
    .AddActor<TestActor>());

builder.Services.AddSingleton<ActorOutput>();

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.MapActorsHandlers();

app.Run();
