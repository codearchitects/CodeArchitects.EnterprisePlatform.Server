using System.Reflection;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal interface IStateTypeBuilder
{
  Type BuildOrdinary(Type actorType, IEnumerable<FieldInfo> stateFields);
  Type BuildPolymorphic(Type actorType, IEnumerable<FieldInfo> stateFields);
}
