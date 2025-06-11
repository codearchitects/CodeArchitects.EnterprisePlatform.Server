using ActorApp.Domain;
using CodeArchitects.Platform.Actors.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDaprInfrastructure(options => options.SetConfiguration(builder.Configuration))
  .AddActors(actors => actors
    .AccessPrivates()
    .AddActor<VirtualActor>()
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

#if NET9_0
Console.WriteLine("Executing app with net9 runtime...");
#elif NET8_0
Console.WriteLine("Executing app with net8 runtime...");
#elif NET7_0
Console.WriteLine("Executing app with net7 runtime...");
#endif

app.Run();
