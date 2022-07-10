using CodeArchitects.Platform.Messaging.Bindings;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Implementation;

/// <summary>
/// Implementation of <see cref="IOutputBindingDescriptor"/>
/// </summary>
internal record OutputBindingDescriptor(
  Type MetadataType,
  object MetadataObject) : IOutputBindingDescriptor
{
  public static IReadOnlyCollection<OutputBindingDescriptor> Create(MethodInfo method, ICollection<HandlerDiagnostics> diagnostics)
  {
    object[] returnAttributes = method.ReturnTypeCustomAttributes.GetCustomAttributes(false);
    Dictionary<Type, OutputBindingDescriptor> descriptors = new(returnAttributes.Length);

    foreach (object returnAttribute in returnAttributes)
    {
      IEnumerable<Type> outputMetadataInterfaceTypes = returnAttribute
        .GetType()
        .GetInterfaces()
        .Where(type => typeof(IOutputMetadata).IsAssignableFrom(type) && type != typeof(IOutputMetadata));

      foreach (Type interfaceType in outputMetadataInterfaceTypes)
      {
        OutputBindingDescriptor descriptor = new OutputBindingDescriptor(interfaceType, returnAttribute);
        if (descriptors.ContainsKey(descriptor.MetadataType))
        {
          diagnostics.Add(DuplicateOutputBinding(method.DeclaringType));
          continue;
        }

        descriptors.Add(descriptor.MetadataType, descriptor);
      }
    }

    return descriptors.Values;
  }

  private static HandlerDiagnostics DuplicateOutputBinding(Type concreteType)
    => new(concreteType, "Duplicate output binding of the same metadata found on handler {0}.", concreteType);
}
