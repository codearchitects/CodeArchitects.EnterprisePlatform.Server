using CodeArchitects.Platform.Actors.Dapr.Infrastructure;
using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Actors.Scheduling;
using Dapr.Actors;
using Dapr.Actors.Runtime;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace CodeArchitects.Platform.Actors.Dapr.Fixtures;

internal class TestActorState : ActorState
{
  public override int ImplementationId
  {
    get => 0;
    set => throw new InvalidOperationException();
  }
}

internal interface ITestActor
{
}

internal class TestActor : ITestActor
{
  public virtual Task Activity(int arg, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}

internal abstract class TestActorActivity : Activity<TestActor>
{
}

internal class TestActorActivity1 : TestActorActivity
{
  public override int Id => 1;

  public int arg { get; set; }

  public override Task ExecuteAsync(TestActor actor, CancellationToken cancellationToken)
  {
    return actor.Activity(arg, cancellationToken);
  }
}

internal interface ITestActorHost : IActor
{
}

internal class TestActorHost : DaprActorHost<TestActor, TestActorState>, ITestActorHost
{
  public TestActorHost(ActorHost host, IActorManager<TestActor, TestActorState> manager, IImplementationFactory<TestActor, TestActorState> factory)
    : base(host, manager, factory)
  {
  }

  public void SetStateManager(IActorStateManager stateManager)
  {
    StateManager = stateManager;
  }
}

internal class TestActorActivityTypeResolver : DefaultJsonTypeInfoResolver
{
  public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
  {
    JsonTypeInfo info = base.GetTypeInfo(type, options);

    if (info.Type == typeof(TestActorActivity))
    {
      info.PolymorphismOptions = new JsonPolymorphismOptions
      {
        TypeDiscriminatorPropertyName = "$activity",
        IgnoreUnrecognizedTypeDiscriminators = true,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        DerivedTypes =
        {
          new JsonDerivedType(typeof(TestActorActivity1), 1)
        }
      };
    }

    return info;
  }
}
