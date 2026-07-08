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
  public bool IsTypeFiltered => typeof(ITypedOutputMetadata).IsAssignableFrom(MetadataType);

  public static IReadOnlyCollection<OutputBindingDescriptor> Create(MethodInfo method)
  {
    object[] returnAttributes = method.ReturnTypeCustomAttributes.GetCustomAttributes(false);
    List<OutputBindingDescriptor> descriptors = new(returnAttributes.Length);

    foreach (object returnAttribute in returnAttributes)
    {
      IEnumerable<Type> outputMetadataInterfaceTypes = returnAttribute
        .GetType()
        .GetInterfaces()
        .Where(type => typeof(IOutputMetadata).IsAssignableFrom(type) && type != typeof(IOutputMetadata) && type != typeof(ITypedOutputMetadata));

      foreach (Type interfaceType in outputMetadataInterfaceTypes)
      {
        OutputBindingDescriptor descriptor = new(interfaceType, returnAttribute);
        descriptors.Add(descriptor);
      }
    }

    return descriptors;
  }
}
