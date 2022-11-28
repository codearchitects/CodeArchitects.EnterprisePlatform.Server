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
    TypeBuilder type = _module.DefineType(
      name: entity.Type.GetComponentTypeName("EqualityComparer"),
      attr: TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: typeof(object),
      interfaces: new[] { typeof(IEqualityComparer<>).MakeGenericType(entity.Type) });

    ImplementEqualsMethod(type, entity);

    return type.CreateTypeInfo()!;
  }

  private static MethodInfo ImplementEqualsMethod(TypeBuilder type, IEntityModel entity)
  {
    MethodInfo declaration = typeof(IEqualityComparer<>).MakeGenericType(entity.Type).GetRequiredMethod(
      name: nameof(IEqualityComparer<object>.Equals),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { entity.Type, entity.Type });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);
    ILGenerator il = method.GetILGenerator();

    bool firstIteration = true;
    foreach (IPrimaryKeyPropertyModel property in entity.PrimaryKey.Properties)
    {
      MethodInfo equalsMethod = property.Type.GetRequiredMethod(
        name: nameof(object.Equals),
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: new[] { property.Type });

      // TODO: Support fields
      Debug.Assert(property.MemberAccess is MemberAccess.Property);

      il.LoadArg(0);
      il.Emit(OpCodes.Call, property.Property!.GetMethod);
      il.LoadArg(1);
      il.Emit(OpCodes.Call, property.Property!.GetMethod);
      il.Emit(OpCodes.Call, equalsMethod);
      if (!firstIteration)
      {
        il.Emit(OpCodes.And);
      }

      firstIteration = false;
    }

    il.Emit(OpCodes.Ret);

    return method;
  }

  private static MethodInfo ImplementGetHashCodeMethod(TypeBuilder type, IEntityModel entity)
  {
    IPrimaryKeyModel primaryKey = entity.PrimaryKey;

    MethodInfo declaration = typeof(IEqualityComparer<>).MakeGenericType(entity.Type).GetRequiredMethod(
      name: nameof(IEqualityComparer<object>.Equals),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { entity.Type, entity.Type });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Final);
    ILGenerator il = method.GetILGenerator();

    foreach (IPrimaryKeyPropertyModel property in primaryKey.Properties)
    {
      // TODO: Support fields
      Debug.Assert(property.MemberAccess is MemberAccess.Property);
      
      il.LoadArg(0);
      il.Emit(OpCodes.Call, property.Property!.GetMethod);
    }

    if (primaryKey.IsComposite)
    {
      ConstructorInfo constructor = primaryKey.Type.GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: primaryKey.Type.GetGenericArguments());

      il.Emit(OpCodes.Newobj, constructor);
    }

    MethodInfo getHashCodeMethod = primaryKey.Type.GetRequiredMethod(
      name: nameof(object.GetHashCode),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    il.Emit(OpCodes.Call, getHashCodeMethod);

    il.Emit(OpCodes.Ret);

    return method;
  }
}
