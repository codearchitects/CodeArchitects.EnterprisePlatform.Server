using System.Reflection;

namespace CodeArchitects.Platform.Actors.Fixtures;

internal class StateTypeDelegator : TypeDelegator
{
  private readonly string _discriminatorPropertyName;

  public StateTypeDelegator(Type stateType, string discriminatorPropertyName)
		: base(stateType)
	{
    _discriminatorPropertyName = discriminatorPropertyName;
  }

  public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
  {
    name = name.Replace("$discriminator", _discriminatorPropertyName);

    return base.GetField(name, bindingAttr);
  }
}
