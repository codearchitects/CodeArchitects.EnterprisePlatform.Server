using ActorApp.Domain;
using CodeArchitects.Platform.Actors.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDaprInfrastructure(options => options.SetConfiguration(builder.Configuration))
  .AddActors(actors => actors
    .AccessPrivates()
    .AddActor<TestActor>())
  .AddMessaging(messaging => messaging
    .Configure(config => config.DefaultBus = "messagebus")
    .AddMessage<TestMessage>()
    .ScanAssembly(ActorMessaging.Assembly));

builder.Services.AddSingleton<ActorOutput>();

var app = builder.Build();

app.UseRouting();

app.MapControllers();

app.MapActorsHandlers();

app.MapMessageHandlers();

app.Run();
