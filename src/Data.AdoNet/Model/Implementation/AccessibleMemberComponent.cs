using CodeArchitects.Platform.Common.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class AccessibleMemberComponent : MemberComponent, IAccessibleMemberModel
{
  private string? _name;

  protected AccessibleMemberComponent(Getter<object?> getValue, Setter<object?> setValue)
  {
    GetValue = getValue;
    SetValue = setValue;
  }

  [AllowNull]
  public string Name
  {
    get => _name ?? Member.Name;
    set => _name = value;
  }

  public new abstract MemberInfo Member { get; }

  public new Getter<object?> GetValue { get; }

  public new Setter<object?> SetValue { get; }

  protected override MemberInfo? MemberCore => Member;

  protected override Getter<object?>? GetValueCore => GetValue;

  protected override Setter<object?>? SetValueCore => SetValue;

  protected override bool HasMemberCore => true;

  public static AccessibleMemberComponent Create(MemberInfo member)
  {
    return member switch
    {
      PropertyInfo property => PropertyMemberComponent.Create(property),
      FieldInfo field       => FieldMemberComponent.Create(field),
      _                     => throw Errors.Unreacheable
    };
  }
}
