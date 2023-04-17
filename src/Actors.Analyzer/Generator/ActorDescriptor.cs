using CodeArchitects.Platform.Actors.Analyzer.Utils;

namespace CodeArchitects.Platform.Actors.Analyzer.Generator;

internal record ActorDescriptor(
  string Namespace,
  string ImplementationTypeName,
  string InterfaceTypeFullName,
  string IdTypeFullName,
  bool IsVirtual,
  bool GetsIdFromState,
  RecordList<StateDependencyDescriptor> StateDependencies);
