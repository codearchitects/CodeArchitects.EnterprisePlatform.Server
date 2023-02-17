using CodeArchitects.Platform.Actors.Infrastructure;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Implementation;

internal class StateDescriptor<TState> : IStateDescriptor<TState>
  where TState : ActorState
{
  [DisallowNull]
  public TState? DefaultValue { get; set; }

  public Type Type => typeof(TState);

  public IReadOnlyList<FieldInfo> Fields => Type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

  object? IStateDescriptor.DefaultValue => DefaultValue;
}
