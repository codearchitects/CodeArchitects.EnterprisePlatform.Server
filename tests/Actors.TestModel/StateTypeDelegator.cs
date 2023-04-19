using System.Reflection;

namespace CodeArchitects.Platform.Actors.TestModel;

internal class StateTypeDelegator : TypeDelegator
{
	public StateTypeDelegator(Type stateType)
		: base(stateType)
	{
	}

  protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
  {
		if (int.TryParse(name, out _))
		{
			name = '_' + name;
		}

		return base.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
  }
}
