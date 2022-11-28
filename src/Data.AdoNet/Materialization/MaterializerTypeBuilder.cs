using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using CodeArchitects.Platform.Emit;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class MaterializerTypeBuilder
{
  private static readonly IReadOnlyDictionary<Type, MethodInfo> s_getValueMethods;
  private static readonly MethodInfo s_getModelMethod;
  private static readonly MethodInfo s_getIndexMethod;
  private static readonly MethodInfo s_createListMethod;
  private static readonly MethodInfo s_createHashSetMethod;

  static MaterializerTypeBuilder()
  {
    s_getValueMethods = new Dictionary<Type, MethodInfo>()
    {
      [typeof(bool)]      = GetGetValueMethod("Boolean"),
      [typeof(byte)]      = GetGetValueMethod("Byte"),
      [typeof(char)]      = GetGetValueMethod("Char"),
      [typeof(DateTime)]  = GetGetValueMethod("DateTime"),
      [typeof(decimal)]   = GetGetValueMethod("Decimal"),
      [typeof(double)]    = GetGetValueMethod("Double"),
      [typeof(float)]     = GetGetValueMethod("Float"),
      [typeof(Guid)]      = GetGetValueMethod("Guid"),
      [typeof(short)]     = GetGetValueMethod("Int16"),
      [typeof(int)]       = GetGetValueMethod("Int32"),
      [typeof(long)]      = GetGetValueMethod("Int64"),
      [typeof(string)]    = GetGetValueMethod("String"),
      [typeof(bool?)]     = GetGetNullableValueMethod("Boolean"),
      [typeof(byte?)]     = GetGetNullableValueMethod("Byte"),
      [typeof(char?)]     = GetGetNullableValueMethod("Char"),
      [typeof(DateTime?)] = GetGetNullableValueMethod("DateTime"),
      [typeof(decimal?)]  = GetGetNullableValueMethod("Decimal"),
      [typeof(double?)]   = GetGetNullableValueMethod("Double"),
      [typeof(float?)]    = GetGetNullableValueMethod("Float"),
      [typeof(Guid?)]     = GetGetNullableValueMethod("Guid"),
      [typeof(short?)]    = GetGetNullableValueMethod("Int16"),
      [typeof(int?)]      = GetGetNullableValueMethod("Int32"),
      [typeof(long?)]     = GetGetNullableValueMethod("Int64")
    };

    s_getModelMethod = typeof(INavigation).GetRequiredMethod($"get_{nameof(INavigation.Model)}");
    s_getIndexMethod = typeof(INavigationModel).GetRequiredMethod($"get_{nameof(INavigationModel.Index)}");
    s_createListMethod = typeof(IMaterializerHub).GetRequiredMethod(
      name: nameof(IMaterializerHub.CreateList),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);
    s_createHashSetMethod = typeof(IMaterializerHub).GetRequiredMethod(
      name: nameof(IMaterializerHub.CreateHashSet),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    static MethodInfo GetGetValueMethod(string name) => typeof(DbDataReader).GetRequiredMethod(
      name: $"Get{name}",
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    static MethodInfo GetGetNullableValueMethod(string name) => typeof(DbDataReaderExtensions).GetRequiredMethod(
      name: $"GetNullable{name}",
      bindingAttr: BindingFlags.Static | BindingFlags.NonPublic,
      types: new[] { typeof(DbDataReader), typeof(int) });
  }

  private readonly ModuleBuilder _module;

  public MaterializerTypeBuilder(ModuleBuilder module)
  {
    _module = module;
  }

  public Type BuildMaterializerType(IEntityModel entity)
  {
    Type baseType = typeof(Materializer<,>).MakeGenericType(entity.Type.UnderlyingSystemType, entity.PrimaryKey.Type.UnderlyingSystemType);

    FieldInfo hubField = baseType.GetRequiredField(
      name: "_hub",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

    TypeBuilder type = _module.DefineType(
      name: entity.Type.GetComponentTypeName("Materializer"),
      attr: TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.Class,
      parent: baseType);

    IReadOnlyDictionary<int, FieldInfo> materializerFields = entity.Navigations.ToDictionary(
      keySelector: navigation => navigation.Id,
      elementSelector: navigation => DefineNavigationMaterializerField(type, navigation));

    OverrideReadKeyMethod(type, entity.PrimaryKey);

    OverrideReadEntityMethod(type, entity.Initializer);

    OverrideReadNavigationMethod(type, hubField, materializerFields, entity);

    return type.CreateTypeInfo()!;
  }

  private static FieldInfo DefineNavigationMaterializerField(TypeBuilder type, INavigationModel navigation)
  {
    IEntityModel target = navigation.To;

    return type.DefineField(
      fieldName: $"_{navigation.Name}Materializer",
      type: typeof(IMaterializer<,>).MakeGenericType(target.Type.UnderlyingSystemType, target.PrimaryKey.Type.UnderlyingSystemType),
      attributes: FieldAttributes.Private);
  }

  private static MethodInfo OverrideReadKeyMethod(TypeBuilder type, IPrimaryKeyModel primaryKey)
  {
    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "ReadKey",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { typeof(DbDataReader), typeof(int) });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    ILGenerator il = method.GetILGenerator();

    foreach (IPrimaryKeyPropertyModel property in primaryKey.Properties)
    {
      LoadColumnValue(il, property);
    }

    if (primaryKey.IsComposite)
    {
      ConstructorInfo constructor = primaryKey.Type.GetRequiredConstructor(
        bindingAttr: BindingFlags.Instance | BindingFlags.Public,
        types: primaryKey.Properties.Map(property => property.Type));

      il.Emit(OpCodes.Newobj, constructor);
    }

    il.Emit(OpCodes.Ret);

    return method;
  }

  private static MethodInfo OverrideReadEntityMethod(TypeBuilder type, IInitializerModel initializer)
  {
    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "ReadEntity",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { typeof(DbDataReader), typeof(int) });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    ILGenerator il = method.GetILGenerator();

    foreach (IPropertyModel property in initializer.ConstructorProperties)
    {
      LoadColumnValue(il, property);                                       // Load column value         | Stack: ...values
    }                                                                      //                           | 
    il.Emit(OpCodes.Newobj, initializer.Constructor);                      // Call new TEntity=$entity  | Stack: $entity
                                                                           //                           | 
    foreach (IPropertyModel property in initializer.InitializerProperties) //                           | 
    { // TODO: Support fields                                              //                           | 
      Debug.Assert(property.MemberAccess is MemberAccess.Property);        //                           | 
      il.Emit(OpCodes.Dup);                                                // Duplicate entity          | Stack: $entity, $entity
      LoadColumnValue(il, property);                                       // Load column value         | Stack: $entity, $entity, $value
      il.Emit(OpCodes.Call, property.Property!.SetMethod!);                // Set property              | Stack: $entity
    }                                                                      //                           | 
    il.Emit(OpCodes.Ret);                                                  // Return entity             | Stack: -

    return method;
  }

  private static MethodInfo OverrideReadNavigationMethod(TypeBuilder type, FieldInfo hubField, IReadOnlyDictionary<int, FieldInfo> materializerFields, IEntityModel entity)
  {
    IReadOnlyList<INavigationModel> navigations = entity.Navigations;

    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "ReadEntity",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { typeof(DbDataReader), typeof(int).MakeByRefType(), entity.Type, typeof(INavigation) });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    ILGenerator il = method.GetILGenerator();

    Label[] cases = Enumerable
      .Range(0, navigations.Count)
      .Select(_ => il.DefineLabel())
      .ToArray();
    Label readLabel = il.DefineLabel();

    il.LoadArg(1); // $reader
    il.Emit(OpCodes.Ldarga_S, 1); // $reader, &offset
    il.LoadArg(4); // $reader, &offset, $navigation
    il.LoadArg(4);                               // $reader, &offset, $navigation, $navigation
    il.Emit(OpCodes.Callvirt, s_getModelMethod); // $reader, &offset, $navigation, $model
    il.Emit(OpCodes.Callvirt, s_getIndexMethod); // $reader, &offset, $navigation, $index
    il.Emit(OpCodes.Switch, cases);              // $reader, &offset, $navigation

    for (int i = 0; i < navigations.Count; i++)
    {
      INavigationModel navigation = navigations[i];

      string methodName = navigation.IsCollection
        ? "ReadReferenceNavigation"
        : "ReadCollectionNavigation";
      Type lastParameterType = navigation.IsCollection
        ? typeof(IIdentityCollection<>).MakeGenericType(navigation.To.Type.UnderlyingSystemType)
        : navigation.To.Type.UnderlyingSystemType.MakeByRefType();

      MethodInfo readNavigationMethod = type.BaseType!.GetRequiredMethod(
        name: methodName,
        bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
        types: new[] {
          typeof(DbDataReader),
          typeof(int).MakeByRefType(),
          typeof(INavigation),
          typeof(IMaterializer<,>).MakeGenericType(navigation.To.Type.UnderlyingSystemType, navigation.To.PrimaryKey.Type.UnderlyingSystemType).MakeByRefType(),
          lastParameterType
        });

      // TODO: Support fields
      Debug.Assert(navigation.MemberAccess is MemberAccess.Property);
      MethodInfo createCollectionMethod = GetCreateCollectionMethod(navigation.Type);

      il.MarkLabel(cases[i]);
      il.Emit(OpCodes.Ldflda, materializerFields[navigation.Id]); // $reader, &offset, $navigation, &materializer
      if (navigation.IsCollection)
      {
        il.LoadArg(3);                                         // $reader, &offset, $navigation, &materializer, $entity
        il.Emit(OpCodes.Call, navigation.Property!.GetMethod); // $reader, &offset, $navigation, &materializer, $property
        il.Emit(OpCodes.Brtrue_S, readLabel);                  // $reader, &offset, $navigation, &materializer

        il.LoadArg(3);                                         // $reader, &offset, $navigation, &materializer, $entity
        il.LoadFields(hubField);                               // $reader, &offset, $navigation, &materializer, $entity, $hub
        il.Emit(OpCodes.Callvirt, createCollectionMethod);     // $reader, &offset, $navigation, &materializer, $entity, $collection
        il.Emit(OpCodes.Call, navigation.Property!.SetMethod); // $reader, &offset, $navigation, &materializer
      }

      il.MarkLabel(readLabel);
      il.LoadArg(3);                                         // $reader, &offset, $navigation, &materializer, $entity
      il.Emit(OpCodes.Call, navigation.Property!.GetMethod); // $reader, &offset, $navigation, &materializer, $property


    }

    return method;
  }

  private static void LoadColumnValue(ILGenerator il, IPropertyModel property)
  {
    MethodInfo readMethod = GetReadMethod(s_getValueMethods, property.Type.UnderlyingSystemType);
    OpCode callOpcode = readMethod.IsStatic ? OpCodes.Call : OpCodes.Callvirt;

    il.LoadArg(1);                   // Push $dataReader                | Stack: ..., $dataReader
    il.LoadInt(property.Index);      // Push ordinal                    | Stack: ..., $dataReader, ordinal
    il.LoadArg(2);                   // Push $offest                    | Stack, ..., $dataReader, ordinal, $offset
    il.Emit(OpCodes.Add);            // Add ordinal + $offset = $index  | Stack, ..., $dataReader, $index
    il.Emit(callOpcode, readMethod); // Call readMethod=$value          | Stack: ..., $value
  }

  private static MethodInfo GetReadMethod(IReadOnlyDictionary<Type, MethodInfo> getValueMethods, Type type)
  {
    if (type.IsEnum)
      return GetReadMethod(getValueMethods, type.GetEnumUnderlyingType());

    if (getValueMethods.TryGetValue(type, out MethodInfo method))
      return method;

    throw new NotSupportedException($"Property type '{type.Name}' is not supported.");
  }

  private static MethodInfo GetCreateCollectionMethod(Type navigationType)
  {
    if (!navigationType.IsGenericType)
      throw new InvalidOperationException("Navigation must be a collection.");

    Type typeDefinition = navigationType.GetGenericTypeDefinition();

    if (
      typeDefinition == typeof(IReadOnlyList<>) ||
      typeDefinition == typeof(IList<>) ||
      typeDefinition == typeof(List<>))
      return s_createListMethod;

    if (
      typeDefinition == typeof(IEnumerable<>) ||
      typeDefinition == typeof(IReadOnlyCollection<>) ||
      typeDefinition == typeof(ICollection<>) ||
      typeDefinition == typeof(HashSet<>))
      return s_createHashSetMethod;

    throw new InvalidOperationException("Navigation must be a collection.");
  }
}
