using CodeArchitects.Platform.Common;

namespace CodeArchitects.Platform.Actors;

/// <summary>
/// Indicates that an actor member is a state component and will be provided by the actor's state store.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class StateAttribute : Attribute
{
  /// <summary>
  /// Creates a new <see cref="StateAttribute"/> instance.
  /// </summary>
  public StateAttribute()
  {
  }

  /// <summary>
  /// Specifies the default value of the state component.
  /// </summary>
  public object? Default
  {
    get => DefaultValue.HasValue ? DefaultValue.Value : null;
    set => DefaultValue = value;
  }

  internal Optional<object?> DefaultValue { get; private set; }
}
