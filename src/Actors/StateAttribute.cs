using CodeArchitects.Platform.Common;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that an actor member is a state component and will be provided by the actor's state store.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class StateAttribute : Attribute
{
  public StateAttribute()
  {
  }

  public StateAttribute(string memberName)
  {
    MemberName = memberName;
  }

  public string? MemberName { get; set; }

  public object? Default
  {
    get => DefaultValue.HasValue ? DefaultValue.Value : null;
    set => DefaultValue = value;
  }

  internal Optional<object?> DefaultValue { get; private set; }
}
