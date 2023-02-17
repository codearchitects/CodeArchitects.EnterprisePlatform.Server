using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IActorIdDescriptor
{
  Type Type { get; }

  bool HasIdSource { get; }
  
  int StateIndex { get; }

  PropertyInfo? IdProperty { get; }
}
