using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class JoinColumnMemberComponent : AccessibleMemberComponent<object?>
{
  private static readonly MemberInfo s_member;

  static JoinColumnMemberComponent()
  {
    s_member = typeof(Dictionary<string, object>).GetDefaultMembers()[0];
  }

  public JoinColumnMemberComponent(string name, Type type, Getter<object?> getValue, Setter<object?> setValue)
    : base(getValue, setValue)
  {
    base.Name = name;
    TypeCore = type;
  }

  [AllowNull]
  public override string Name
  {
    get => base.Name;
    set => throw new InvalidOperationException("Cannot set the name of a join column member component.");
  }

  public override MemberInfo Member => s_member;

  protected override Type TypeCore { get; }

  protected override FieldInfo? FieldCore => null;

  protected override PropertyInfo? PropertyCore => null;

  public static JoinColumnMemberComponent Create(string name, Type type)
  {
    Getter<object?> getValue = delegate (object instance)
    {
      var dictionary = (Dictionary<string, object?>)instance;
      return dictionary[name];
    };

    Setter<object?> setValue = delegate (object instance, object? value)
    {
      var dictionary = (Dictionary<string, object?>)instance;
      dictionary[name] = value;
    };

    return new(name, type, getValue, setValue);
  }
}
