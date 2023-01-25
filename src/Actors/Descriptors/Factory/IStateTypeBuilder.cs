using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal interface IStateTypeBuilder
{
  Type Build(Type actorType, IEnumerable<FieldInfo> stateFields);
}
