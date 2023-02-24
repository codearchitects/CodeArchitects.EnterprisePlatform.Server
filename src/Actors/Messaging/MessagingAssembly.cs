using System.Reflection;

namespace CodeArchitects.Platform.Actors.Messaging;

internal class MessagingAssembly : Assembly
{
  private readonly List<Type> _handlerTypes;

  public MessagingAssembly()
  {
    _handlerTypes = new();
  }

  public void AddHandlerType(Type handlerType)
  {
    _handlerTypes.Add(handlerType);
  }

  public override Type[] GetTypes()
  {
    return _handlerTypes.ToArray();
  }
}
