using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class MemberComponent<T>
{
  public Type Type => TypeCore;

  public MemberInfo? Member => MemberCore;

  public FieldInfo? Field => FieldCore;

  public PropertyInfo? Property => PropertyCore;

  public bool HasMember => HasMemberCore;

  public Getter<T>? GetValue => GetValueCore;

  public Setter<T>? SetValue => SetValueCore;

  protected abstract Type TypeCore { get; }

  protected abstract MemberInfo? MemberCore { get; }

  protected abstract FieldInfo? FieldCore { get; }

  protected abstract PropertyInfo? PropertyCore { get; }

  protected abstract bool HasMemberCore { get; }

  protected abstract Getter<T>? GetValueCore { get; }

  protected abstract Setter<T>? SetValueCore { get; }
}
