using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

internal class HandlerDescriptorEqualityComparer :
  IEqualityComparer<IMessagingDescriptor>,
  IEqualityComparer<IHandlerDescriptor>,
  IEqualityComparer<IOutputBindingDescriptor>,
  IEqualityComparer<IMessageDescriptor>,
  IEqualityComparer<HandlerDiagnostics>,
  IEqualityComparer<Type>
{
  public static readonly HandlerDescriptorEqualityComparer Instance = new();

  private HandlerDescriptorEqualityComparer() { }

  public bool Equals(IMessagingDescriptor? x, IMessagingDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.HandlerDescriptors.SequenceEqual(y.HandlerDescriptors, this))
      return false;

    if (!x.MessageDescriptors.SequenceEqual(y.MessageDescriptors, this))
      return false;

    return true;
  }

  public bool Equals(IHandlerDescriptor? x, IHandlerDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.Bus != y.Bus)
      return false;

    if (x.Topic != y.Topic)
      return false;

    if (!Equals(x.InterfaceType, y.InterfaceType))
      return false;

    if (!Equals(x.ConcreteType, y.ConcreteType))
      return false;

    if (!Equals(x.MessageType, y.MessageType))
      return false;

    if (!Equals(x.ResultType, y.ResultType))
      return false;

    if (x.HasResult != y.HasResult)
      return false;

    if (x.HasUnionResult != y.HasUnionResult)
      return false;

    if (!x.ResultTypes.SequenceEqual(y.ResultTypes))
      return false;

    if (!x.OutputBindingDescriptors.SequenceEqual(y.OutputBindingDescriptors, this))
      return false;

    return true;
  }

  public bool Equals(IOutputBindingDescriptor? x, IOutputBindingDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.MetadataType != y.MetadataType)
      return false;

    if (x.IsTypeFiltered != y.IsTypeFiltered)
      return false;

    if (!x.MetadataObject.Equals(y.MetadataObject))
      return false;

    return true;
  }

  public bool Equals(HandlerDiagnostics? x, HandlerDiagnostics? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return x.MessageTemplate == y.MessageTemplate;
  }

  public bool Equals(IMessageDescriptor? x, IMessageDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    return (x.Name, x.Type) == (y.Name, y.Type);
  }

  public bool Equals(Type? x, Type? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (!x.IsGenericType)
    {
      if (y.IsGenericType)
        return false;

      return x.Equals(y);
    }

    if (x.GetGenericTypeDefinition() != y.GetGenericTypeDefinition())
      return false;

    return x.GetGenericArguments().SequenceEqual(y.GetGenericArguments(), this);
  }

  public int GetHashCode([DisallowNull] IMessagingDescriptor obj)
  {
    HashCode hashCode = new();

    foreach (IHandlerDescriptor handlerDescriptor in obj.HandlerDescriptors)
    {
      hashCode.Add(handlerDescriptor, this);
    }
    foreach (IMessageDescriptor messageDescriptor in obj.MessageDescriptors)
    {
      hashCode.Add(messageDescriptor, this);
    }

    return hashCode.ToHashCode();
  }

  public int GetHashCode([DisallowNull] IHandlerDescriptor obj)
  {
    HashCode hashCode = new();

    hashCode.Add(obj.InterfaceType);
    hashCode.Add(obj.MessageType);
    hashCode.Add(obj.ResultType);
    hashCode.Add(obj.Bus);
    hashCode.Add(obj.Topic);
    foreach (IOutputBindingDescriptor descriptor in obj.OutputBindingDescriptors)
    {
      hashCode.Add(descriptor, this);
    }

    return hashCode.ToHashCode();
  }

  public int GetHashCode([DisallowNull] IOutputBindingDescriptor obj)
  {
    return HashCode.Combine(obj.MetadataType, obj.MetadataObject);
  }

  public int GetHashCode([DisallowNull] HandlerDiagnostics obj)
  {
    return obj.MessageTemplate.GetHashCode();
  }

  public int GetHashCode([DisallowNull] IMessageDescriptor obj)
  {
    return HashCode.Combine(obj.Name, obj.Type);
  }

  public int GetHashCode([DisallowNull] Type obj)
  {
    return 0;
  }
}
