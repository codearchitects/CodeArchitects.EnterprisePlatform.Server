using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal class HiddenMemberComponent : MemberComponent
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

  protected override Getter<object?>? GetValueCore => null;

  protected override Setter<object?>? SetValueCore => null;
}
