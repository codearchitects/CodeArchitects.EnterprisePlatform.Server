using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors;

internal class HandlerDiagnostics
{
  private HandlerDiagnostics(int id, Type concreteType, string errorTemplate, params object?[] errorArguments)
  {
    Id = id;
    ConcreteType = concreteType;
    ErrorTemplate = errorTemplate;
    ErrorArguments = errorArguments;
  }

  public int Id { get; }
  public Type ConcreteType { get; }
  public string ErrorTemplate { get; }
  public object?[] ErrorArguments { get; }

  public static HandlerDiagnostics MultipleMessageHandlerAttributeOnClass(Type concreteType)
    => new(1, concreteType, $"Multiple {nameof(MessageHandlerAttribute)} found on type {{0}}", concreteType);

  public static HandlerDiagnostics NullBusOnHandler(Type concreteType, MethodInfo method)
    => new(2, concreteType, "Handler method {0} on type {1} had null bus name for some subscription", method, concreteType);

  public static HandlerDiagnostics NullTopicOnHandler(Type concreteType, MethodInfo method)
    => new(3, concreteType, "Handler method {0} on type {1} had null topic name for some subscription", method, concreteType);
}
