using CodeArchitects.Platform.Actors.Infrastructure;
using CodeArchitects.Platform.Emit;
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

  public Type Build(Type actorType, IEnumerable<IStateComponentMetadata> components, bool isPolymorphic)
  {
    Type baseType = isPolymorphic ? typeof(PolymorphicActorState) : typeof(OrdinaryActorState);

    TypeBuilder type = _module.DefineType(
      name: actorType.GetComponentTypeName(ComponentName),
      attr: TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType);

    foreach (IStateComponentMetadata component in components)
    {
      BuildAutoProperty(type, component.Index.ToString(), component.Type);
    }

    return type.CreateTypeInfo()!;
  }
}