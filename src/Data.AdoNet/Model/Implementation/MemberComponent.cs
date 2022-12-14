using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class MemberComponent<T>
{
  private object? _defaultValue;

  public Type Type => TypeCore;

  public MemberInfo? Member => MemberCore;

  public FieldInfo? Field => FieldCore;

  public PropertyInfo? Property => PropertyCore;

  [MemberNotNullWhen(true, nameof(Member), nameof(GetValue), nameof(SetValue))]
  public bool HasMember => HasMemberCore;

  public Getter<T>? GetValue => GetValueCore;

  public Setter<T>? SetValue => SetValueCore;

  public object? DefaultValue => _defaultValue ??= CreateDefaultValue();

  protected abstract Type TypeCore { get; }

  protected abstract MemberInfo? MemberCore { get; }

  protected abstract FieldInfo? FieldCore { get; }

  protected abstract PropertyInfo? PropertyCore { get; }

  protected abstract bool HasMemberCore { get; }

  protected abstract Getter<T>? GetValueCore { get; }

  protected abstract Setter<T>? SetValueCore { get; }

  private object? CreateDefaultValue()
  {
    return Type.IsValueType
      ? Activator.CreateInstance(Type)
      : null;
  }
}
