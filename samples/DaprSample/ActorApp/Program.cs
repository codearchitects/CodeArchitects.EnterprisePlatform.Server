using ActorApp.Domain;
using ActorApp.OldSkool;
using CodeArchitects.Platform.Actors.Dapr.Factory;
using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Dapr.Proxy;
using CodeArchitects.Platform.Actors.Descriptors;
using CodeArchitects.Platform.Actors.Descriptors.Reflection;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using CodeArchitects.Platform.Emit;
using CodeArchitects.Platform.Emit.Reflection;
using Dapr.Actors.Runtime;

var builder = WebApplication.CreateBuilder(args);

DynamicAssembly.IgnoreAccessCheckTo("ActorApp");
ReflectionMetadataContext context = new();
context.AddAssembly(typeof(TrafficLight).Assembly);
IActorModel model = context.CreateModel();
IActorDescriptor descriptor = model.GetActor(typeof(TrafficLight));

ActorHostTypeBuilder actorHostTypeBuilder = new(DynamicAssembly.Module, new ReflectionILGeneratorProvider());
ActorProxyTypeBuilder actorProxyTypeBuilder = new(DynamicAssembly.Module, new ReflectionILGeneratorProvider());
DaprProxyFactoryTypeBuilder proxyFactoryTypeBuilder = new(DynamicAssembly.Module, new ReflectionILGeneratorProvider());

ActorHostEmitResult hostEmitResult = actorHostTypeBuilder.Build(descriptor, null);
Type proxyType = actorProxyTypeBuilder.Build(descriptor, hostEmitResult);
Type actorFactoryType = proxyFactoryTypeBuilder.Build(descriptor, hostEmitResult, proxyType, null);

IActivityManager<TrafficLight> activityManager = ActivityManager<TrafficLight>.Create(descriptor);

builder.Services.AddControllers();
builder.Services.AddDaprClient();
builder.Services.AddActors(options =>
{
  options.Actors.Add(new ActorRegistration(ActorTypeInformation.Get(hostEmitResult.ClassType, nameof(TrafficLight))));
  options.Actors.RegisterActor<TrafficLightActor>();
});
builder.Services.AddSingleton(typeof(ITrafficLightFactory), actorFactoryType);
builder.Services.AddSingleton(model);
builder.Services.AddSingleton(activityManager);
builder.Services.AddScoped(typeof(IManagerFactory<,>), typeof(ManagerFactory<,>));

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
