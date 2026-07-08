using System.Reflection;

namespace CodeArchitects.Platform.Actors.Dapr.Fixtures;

internal class ActorHostTypeDelegator : TypeDelegator
{
	public ActorHostTypeDelegator(Type hostType)
		: base(hostType)
	{
	}

  protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
  {
    if (name == Constants.InitAsyncMethodName)
    {
      name = "_InitAsync";
    }

    return base.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
  }
}
