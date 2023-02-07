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

  public Type Build(Type actorType, IEnumerable<FieldInfo> stateFields, bool isPolymorphic)
  {
    Debug.Assert(stateFields.Count() > 0 || isPolymorphic, "Expected at least one state component or a polymorphic actor.");

    TypeBuilder type = _module.DefineType(
      name: actorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class);

    foreach (FieldInfo stateField in stateFields)
    {
      BuildAutoProperty(type, stateField.Name, stateField.FieldType);
    }

    if (isPolymorphic)
    {
      BuildAutoProperty(type, "$discriminator", typeof(string));
    }

    return type.CreateTypeInfo()!;
  }
}