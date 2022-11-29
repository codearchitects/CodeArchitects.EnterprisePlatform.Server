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
  private static readonly ConstructorInfo s_invalidOperationExceptionConstructor;

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
    
    s_createListMethod = typeof(IIdentityCollectionFactory).GetRequiredMethod(
      name: nameof(IMaterializerHub.CreateList),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);
    
    s_createHashSetMethod = typeof(IIdentityCollectionFactory).GetRequiredMethod(
      name: nameof(IMaterializerHub.CreateHashSet),
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: Type.EmptyTypes);

    s_invalidOperationExceptionConstructor = typeof(InvalidOperationException).GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(string) });

    static MethodInfo GetGetValueMethod(string name) => typeof(DbDataReader).GetRequiredMethod(
      name: $"Get{name}",
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    static MethodInfo GetGetNullableValueMethod(string name) => typeof(DbDataReaderExtensions).GetRequiredMethod(
      name: $"GetNullable{name}",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(DbDataReader), typeof(int) });
  }

  private readonly ModuleBuilder _module;

  public MaterializerTypeBuilder(ModuleBuilder module)
  {
    _module = module;
  }

  public Type Build(IEntityModel entity)
  {
    Type baseType = typeof(Materializer<,>).MakeGenericType(entity.Type, entity.PrimaryKey.Type);

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

    DefineConstructor(type);

    OverridePropertyCountProperty(type, entity);

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
      type: typeof(IMaterializer<,>).MakeGenericType(target.Type, target.PrimaryKey.Type),
      attributes: FieldAttributes.Private);
  }

  private static ConstructorInfo DefineConstructor(TypeBuilder type)
  {
    ConstructorInfo baseConstructor = type.BaseType!.GetRequiredConstructor(
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { typeof(IMaterializerHub) });

    ConstructorBuilder constructor = type.DefineConstructor(
      attributes: MethodAttributes.Public,
      callingConvention: CallingConventions.HasThis,
      parameterTypes: new[] { typeof(IMaterializerHub) });

    ILGenerator il = constructor.GetILGenerator();

    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Call, baseConstructor);
    il.Emit(OpCodes.Ret);

    return constructor;
  }

  private static MethodInfo OverridePropertyCountProperty(TypeBuilder type, IEntityModel entity)
  {
    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: $"get_PropertyCount",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: Type.EmptyTypes);

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    ILGenerator il = method.GetILGenerator();

    il.LoadInt(entity.Properties.Count);
    il.Emit(OpCodes.Ret);

    return method;
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
    IReadOnlyList<INavigationModel> navigations = entity.Navigations
      .Where(nav => nav.MemberAccess is not MemberAccess.None)
      .ToList();

    MethodInfo declaration = type.BaseType!.GetRequiredMethod(
      name: "ReadNavigation",
      bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic,
      types: new[] { typeof(DbDataReader), typeof(int).MakeByRefType(), entity.Type, typeof(INavigation) });

    MethodBuilder method = type.DefineMethodOverrideFromDeclaration(declaration, MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.Final);
    ILGenerator il = method.GetILGenerator();

    Label[] cases = Enumerable
      .Range(0, navigations.Count)
      .Select(_ => il.DefineLabel())
      .ToArray();
    Label throwLabel = il.DefineLabel();

    il.LoadArg(4);                               // $navigation
    il.Emit(OpCodes.Callvirt, s_getModelMethod); // $model
    il.Emit(OpCodes.Callvirt, s_getIndexMethod); // $index
    il.Emit(OpCodes.Switch, cases);              // -
    il.Emit(OpCodes.Br_S, throwLabel);

    for (int i = 0; i < navigations.Count; i++)
    {
      INavigationModel navigation = navigations[i];

      // TODO: Support fields
      Debug.Assert(navigation.MemberAccess is MemberAccess.Property);

      MethodInfo materializeNavigationMethod = type.BaseType!.GetRequiredMethod(
        name: "MaterializeNavigation",
        bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic)
        .MakeGenericMethod(navigation.To.Type, navigation.To.PrimaryKey.Type);

      il.MarkLabel(cases[i]);
      il.LoadArg(3); // $entity
      if (navigation.IsCollection)
      {
        MethodInfo createCollectionMethod = GetCreateCollectionMethod(navigation.Type);
        Label readLabel = il.DefineLabel();
        
        il.Emit(OpCodes.Callvirt, navigation.Property!.GetMethod); // $property
        il.Emit(OpCodes.Brtrue_S, readLabel); // -
        
        il.LoadArg(3); // $entity
        il.LoadFields(hubField); // $entity, $hub
        il.Emit(OpCodes.Callvirt, createCollectionMethod); // $entity, $collection
        il.Emit(OpCodes.Callvirt, navigation.Property!.SetMethod); // -

        il.MarkLabel(readLabel);
        il.LoadArg(3); // $entity
        il.Emit(OpCodes.Callvirt, navigation.Property!.GetMethod); // $property
        il.Emit(OpCodes.Castclass, typeof(IIdentityCollection<>).MakeGenericType(navigation.To.Type)); // $property
      }

      il.LoadArg(0); // $entity/$property, $this
      il.LoadArg(1); // $entity/$property, $this, $reader
      il.LoadArg(2); // $entity/$property, $this, $reader, &offset
      il.LoadArg(4); // $entity/$property, $this, $reader, &offset, $navigation
      il.LoadArg(0); // $entity/$property, $this, $reader, &offset, $navigation, $this
      il.Emit(OpCodes.Ldflda, materializerFields[navigation.Id]); // $entity/$property, $this, $reader, &offset, $navigation, &materializer

      il.Emit(OpCodes.Call, materializeNavigationMethod); // $entity/$property, $navigationEntity
      if (navigation.IsCollection)
      {
        MethodInfo addEntityMethod = typeof(IIdentityCollection<>).MakeGenericType(navigation.To.Type).GetRequiredMethod(
          name: nameof(IIdentityCollection<object>.AddEntity),
          bindingAttr: BindingFlags.Instance | BindingFlags.Public,
          types: new[] { navigation.To.Type });

        il.Emit(OpCodes.Callvirt, addEntityMethod); // -
      }
      else
      {
        il.Emit(OpCodes.Callvirt, navigation.Property!.SetMethod); // -
      }

      il.Emit(OpCodes.Ret);
    }

    il.MarkLabel(throwLabel);
    il.Emit(OpCodes.Ldstr, "Invalid navigation.");
    il.Emit(OpCodes.Newobj, s_invalidOperationExceptionConstructor);
    il.Emit(OpCodes.Throw);

    return method;
  }

  private static void LoadColumnValue(ILGenerator il, IPropertyModel property)
  {
    MethodInfo readMethod = GetReadMethod(s_getValueMethods, property.Type);
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
      return s_createListMethod.MakeGenericMethod(navigationType.GetGenericArguments());

    if (
      typeDefinition == typeof(IEnumerable<>) ||
      typeDefinition == typeof(IReadOnlyCollection<>) ||
      typeDefinition == typeof(ICollection<>) ||
      typeDefinition == typeof(HashSet<>))
      return s_createHashSetMethod.MakeGenericMethod(navigationType.GetGenericArguments());

    throw new InvalidOperationException("Navigation must be a collection.");
  }
}
