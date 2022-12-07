using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class MemberComponent : IMemberModel
{
  public Type Type => TypeCore;

  public MemberInfo? Member => MemberCore;

  public FieldInfo? Field => FieldCore;

  public PropertyInfo? Property => PropertyCore;

  public bool HasMember => HasMemberCore;

  public Getter<object?>? GetValue => GetValueCore;

  public Setter<object?>? SetValue => SetValueCore;

  protected abstract Type TypeCore { get; }

  protected abstract MemberInfo? MemberCore { get; }

  protected abstract FieldInfo? FieldCore { get; }

  protected abstract PropertyInfo? PropertyCore { get; }

  protected abstract bool HasMemberCore { get; }

  protected abstract Getter<object?>? GetValueCore { get; }

  protected abstract Setter<object?>? SetValueCore { get; }
}
