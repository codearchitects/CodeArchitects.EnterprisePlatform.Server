using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Messaging.Descriptors.Concrete;

internal class HandlerDescriptorEqualityComparer :
  IEqualityComparer<IHandlerDescriptor>,
  IEqualityComparer<IHandlerIdentityDescriptor>,
  IEqualityComparer<IOutputBindingDescriptor>
{
  public static readonly HandlerDescriptorEqualityComparer Instance = new();

  private HandlerDescriptorEqualityComparer() { }

  public bool Equals(IHandlerDescriptor? x, IHandlerDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.ConcreteType != y.ConcreteType)
      return false;

    if (!x.IdentityDescriptors.SequenceEqual(y.IdentityDescriptors, this))
      return false;

    return true;
  }

  public bool Equals(IHandlerIdentityDescriptor? x, IHandlerIdentityDescriptor? y)
  {
    if (x is null)
      return y is null;

    if (y is null)
      return false;

    if (x.InterfaceType != y.InterfaceType)
      return false;

    if (x.MessageType != y.MessageType)
      return false;

    if (x.ResultType != y.ResultType)
      return false;

    if (x.Bus != y.Bus)
      return false;

    if (x.Topic != y.Topic)
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

    if (!x.MetadataObject.Equals(y.MetadataObject))
      return false;

    return true;
  }

  public int GetHashCode([DisallowNull] IHandlerDescriptor obj)
  {
    HashCode hashCode = new();
    
    hashCode.Add(obj.ConcreteType);
    foreach (IHandlerIdentityDescriptor descriptor in obj.IdentityDescriptors)
    {
      hashCode.Add(descriptor, this);
    }

    return hashCode.ToHashCode();
  }

  public int GetHashCode([DisallowNull] IHandlerIdentityDescriptor obj)
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
}
