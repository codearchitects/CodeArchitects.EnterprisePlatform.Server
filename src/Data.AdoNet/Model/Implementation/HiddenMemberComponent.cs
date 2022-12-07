using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenMemberComponent<T> : MemberComponent<T>
{
  public HiddenMemberComponent(Type type)
  {
    TypeCore = type;
  }

  protected override Type TypeCore { get; }

  protected override MemberInfo? MemberCore => null;

  protected override FieldInfo? FieldCore => null;

  protected override PropertyInfo? PropertyCore => null;

  protected override bool HasMemberCore => false;

  protected override Getter<T>? GetValueCore => null;

  protected override Setter<T>? SetValueCore => null;
}
