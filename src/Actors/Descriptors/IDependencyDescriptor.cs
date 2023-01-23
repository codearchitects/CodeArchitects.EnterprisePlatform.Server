using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors;

internal interface IDependencyDescriptor
{
  DependencyKind Kind { get; }

  ParameterInfo Parameter { get; }

  string Name { get; }

  Type Type { get; }

  int Index { get; }

  int CategoryIndex { get; }

  void Accept(IDependencyDescriptorVisitor visitor);
}
