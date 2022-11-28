using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Emit;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class EntityEqualityComparerTypeBuilder
{
  private readonly ModuleBuilder _module;

  public EntityEqualityComparerTypeBuilder(ModuleBuilder module)
  {
    _module = module;
  }

  public Type Build(IEntityModel entity)
  {
    Type baseType = typeof(EntityEqualityComparer<,>).MakeGenericType(entity.Type.UnderlyingSystemType, entity.PrimaryKey.Type.UnderlyingSystemType);

    TypeBuilder type = _module.DefineType(
      name: entity.Type.GetComponentTypeName("EqualityComparer"),
      attr: TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType);

    ImplementGetKeyMethod(type, entity);

    return type.CreateTypeInfo()!;
  }

  private static MethodInfo ImplementGetKeyMethod(TypeBuilder type, IEntityModel entity)
  {
    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "GetKey",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { entity.Type.UnderlyingSystemType });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);

    ILGenerator il = method.GetILGenerator();

    foreach (IPrimaryKeyPropertyModel property in entity.PrimaryKey.Properties)
    {
      il.LoadArg(1);
      il.Emit(OpCodes.Callvirt, property.Property!.GetMethod); // TODO: Support fields
    }

    if (entity.PrimaryKey.IsComposite)
    {
      ConstructorInfo constructor = entity.PrimaryKey.Type.GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: entity.PrimaryKey.Type.GetGenericArguments());

      il.Emit(OpCodes.Newobj, constructor);
    }

    il.Emit(OpCodes.Ret);

    return method;
  }
}
