using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class ActorJsonTypeInfoResolver : DefaultJsonTypeInfoResolver
{
  private readonly IActorDescriptor _actor;

  public ActorJsonTypeInfoResolver(IActorDescriptor actor)
  {
    _actor = actor;
  }

  public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
  {
    JsonTypeInfo info = base.GetTypeInfo(type, options);

    if (info.Type == _actor.ActivityBaseType)
    {
      info.PolymorphismOptions = new JsonPolymorphismOptions
      {
        TypeDiscriminatorPropertyName = ":id",
        IgnoreUnrecognizedTypeDiscriminators = false,
        UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
      };

      foreach (IMethodDescriptor activity in _actor.Activities)
      {
        info.PolymorphismOptions.DerivedTypes.Add(new JsonDerivedType(activity.ActivityType, activity.Id));
      }
    }

    return info;
  }
}
