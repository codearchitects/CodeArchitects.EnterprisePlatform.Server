using System.Reflection;

namespace CodeArchitects.Platform.Actors.Metadata;

internal interface IImplementationMetadata
{
  bool IsDefault { get; }

  Type ImplementationType { get; }

  ConstructorInfo? Constructor { get; }

  bool HasStateFields { get; }

  IMethodMetadata GetMethodMetadata(MethodInfo implementationMethod);
}
