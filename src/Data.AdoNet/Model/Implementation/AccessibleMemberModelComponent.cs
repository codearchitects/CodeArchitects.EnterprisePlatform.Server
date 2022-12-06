using CodeArchitects.Platform.Common.Utils;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class AccessibleMemberModelComponent : MemberModelComponent, IAccessibleMemberModelBase
{
  protected AccessibleMemberModelComponent(Getter<object?> getValue, Setter<object?> setValue)
  {
    GetValue = getValue;
    SetValue = setValue;
  }

  public new abstract MemberInfo Member { get; }

  public new Getter<object?> GetValue { get; }

  public new Setter<object?> SetValue { get; }

  protected override MemberInfo? MemberCore => Member;

  protected override Getter<object?>? GetValueCore => GetValue;

  protected override Setter<object?>? SetValueCore => SetValue;

  protected override bool HasMemberCore => true;

  public static AccessibleMemberModelComponent Create(MemberInfo member)
  {
    return member switch
    {
      PropertyInfo property => PropertyMemberModelComponent.Create(property),
      FieldInfo field       => FieldMemberModelComponent.Create(field),
      _                     => throw Errors.Unreacheable
    };
  }
}
