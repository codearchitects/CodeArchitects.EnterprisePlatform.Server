using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class MemberModel : IMemberModel
{
  protected abstract MemberComponent<object?> MemberComponent { get; }

  public Type Type => MemberComponent.Type;

  public MemberInfo? Member => MemberComponent.Member;

  public FieldInfo? Field => MemberComponent.Field;

  public PropertyInfo? Property => MemberComponent.Property;

  [MemberNotNullWhen(true, nameof(Member), nameof(GetValue), nameof(SetValue))]
  public bool HasMember => MemberComponent.HasMember;

  public Getter<object?>? GetValue => MemberComponent.GetValue;

  public Setter<object?>? SetValue => MemberComponent.SetValue;
}
