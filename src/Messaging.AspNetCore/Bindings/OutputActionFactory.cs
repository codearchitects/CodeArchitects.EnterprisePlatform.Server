using CodeArchitects.Platform.Messaging.Bindings;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

/// <summary>
/// Implementation of <see cref="IOutputActionFactory"/>.
/// </summary>
internal class OutputActionFactory : IOutputActionFactory
{
  public OutputAction CreateOutputAction(Type metadataType, object metadata, IServiceProvider services)
  {
    if (!metadataType.IsInstanceOfType(metadata))
      throw new ArgumentException($"Value of '{nameof(metadata)}' is not assignable to type {metadataType.Name}.", nameof(metadata));

    Type outputActionType = typeof(ITypedOutputMetadata).IsAssignableFrom(metadataType)
      ? typeof(TypedOutputAction<>)
      : typeof(OutputAction<>);

    return (OutputAction)Activator.CreateInstance(outputActionType.MakeGenericType(metadataType), new[] { metadata, services })!;
  }
}
