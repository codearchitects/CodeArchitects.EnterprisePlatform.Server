namespace CodeArchitects.Platform.Messaging.AspNetCore.Bindings;

internal class OutputActionFactory : IOutputActionFactory
{
  public OutputAction CreateOutputAction(Type metadataType, object metadata, IServiceProvider services)
  {
    return metadataType.IsInstanceOfType(metadata)
      ? (OutputAction)Activator.CreateInstance(typeof(OutputAction<>).MakeGenericType(metadataType), new[] { services, metadata })!
      : throw new ArgumentException($"Value of '{nameof(metadata)}' is not assignable to type {metadataType.Name}.", nameof(metadata));
  }
}
