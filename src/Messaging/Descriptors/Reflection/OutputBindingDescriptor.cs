using CodeArchitects.Platform.Messaging.Bindings;
using System.Reflection;

namespace CodeArchitects.Platform.Messaging.Descriptors.Reflection;

/// <summary>
/// Implementation of <see cref="IOutputBindingDescriptor"/>
/// </summary>
internal record OutputBindingDescriptor(
  Type MetadataType,
  object MetadataObject) : IOutputBindingDescriptor
{
  public static IReadOnlyCollection<OutputBindingDescriptor> Create(MethodInfo method)
  {
    object[] returnAttributes = method.ReturnTypeCustomAttributes.GetCustomAttributes(false);
    List<OutputBindingDescriptor> descriptors = new List<OutputBindingDescriptor>(returnAttributes.Length);

    foreach (object returnAttribute in returnAttributes)
    {
      IEnumerable<Type> outputMetadataInterfaceTypes = returnAttribute
        .GetType()
        .GetInterfaces()
        .Where(type => typeof(IOutputMetadata).IsAssignableFrom(type) && type != typeof(IOutputMetadata));

      foreach (Type interfaceType in outputMetadataInterfaceTypes)
      {
        OutputBindingDescriptor descriptor = new OutputBindingDescriptor(interfaceType, returnAttribute);
        descriptors.Add(descriptor);
      }
    }

    return descriptors;
  }
}
