using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Emit;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Actors.Descriptors.Factory;

internal class StateTypeBuilder : TypeBuilderBase, IStateTypeBuilder
{
  public const string ComponentName = "State";

  public StateTypeBuilder(ModuleBuilder module, IILGeneratorProvider ilProvider)
    : base(module, ilProvider)
  {
  }

  public Type BuildOrdinary(Type actorType, IEnumerable<FieldInfo> stateFields)
  {
    return Build(actorType, typeof(OrdinaryActorState), stateFields);
  }

  public Type BuildPolymorphic(Type actorType, IEnumerable<FieldInfo> stateFields)
  {
    return Build(actorType, typeof(PolymorphicActorState), stateFields);
  }

  private Type Build(Type actorType, Type baseType, IEnumerable<FieldInfo> stateFields)
  {
    Debug.Assert(stateFields.Count() > 0, "Expected at least one state component or a polymorphic actor.");

    TypeBuilder type = _module.DefineType(
      name: actorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType);

    foreach (FieldInfo stateField in stateFields)
    {
      BuildAutoProperty(type, stateField.Name, stateField.FieldType);
    }

    return type.CreateTypeInfo()!;
  }
}