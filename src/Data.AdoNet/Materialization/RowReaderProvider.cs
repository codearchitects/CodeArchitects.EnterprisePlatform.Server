using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

internal class RowReaderProvider : IRowReaderProvider
{
  private static readonly IReadOnlyDictionary<Type, MethodInfo> s_getValueMethods;

  static RowReaderProvider()
  {
    s_getValueMethods = new Dictionary<Type, MethodInfo>()
    {
      [typeof(object)]    = GetGetValueMethod("Value"),
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

    EnsureCanReadSupportedTypes(s_getValueMethods);

    static MethodInfo GetGetValueMethod(string name) => typeof(IDataRecord).GetRequiredMethod(
      name: $"Get{name}",
      bindingAttr: BindingFlags.Instance | BindingFlags.Public,
      types: new[] { typeof(int) });

    static MethodInfo GetGetNullableValueMethod(string name) => typeof(DataRecordExtensions).GetRequiredMethod(
      name: $"GetNullable{name}",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      types: new[] { typeof(IDataRecord), typeof(int) });
  }

  private readonly ConcurrentDictionary<IEntityModel, RowReader> _readers;

  public RowReaderProvider(ConcurrentDictionary<IEntityModel, RowReader> readers)
  {
    _readers = readers;
  }

  public IRowReader GetRowReader(IEntityModel entity)
  {
    return _readers.GetOrAdd(entity, CreateRowReader);
  }

  private static RowReader CreateRowReader(IEntityModel entity)
  {
    ParameterExpression readerParam = Expression.Parameter(typeof(IDataReader), "reader");
    ParameterExpression offsetParam = Expression.Parameter(typeof(int), "offset");

    KeyReader keyReader = entity.PrimaryKey.IsComposite
      ? CreateCompositeKeyReader()
      : CreateSimpleKeyReader();

    EntityReader entityReader = CreateEntityReader();

    return new RowReader(keyReader, entityReader, entity, entity.PrimaryKey.Columns[0].Index);

    KeyReader CreateSimpleKeyReader()
    {
      Expression<KeyReader> keyReaderExpr = Expression.Lambda<KeyReader>(
        body: Expression.Call(
          method: s_getValueMethods[typeof(object)],
          instance: readerParam,
          arguments: offsetParam),
        parameters: new[] { readerParam, offsetParam });

      return keyReaderExpr.Compile();
    }

    KeyReader CreateCompositeKeyReader()
    {
      Expression<KeyReader> keyReaderExpr = Expression.Lambda<KeyReader>(
        body: Expression.New(
          constructor: entity.PrimaryKey.TupleConstructor,
          arguments: entity.PrimaryKey.Columns.Select(column => MakeGetValueCallExpression(column.Type, column.Index))),
        parameters: new[] { readerParam, offsetParam });

      return keyReaderExpr.Compile();
    }

    EntityReader CreateEntityReader()
    {
      Expression<EntityReader> readerExpr = Expression.Lambda<EntityReader>(
        body: Expression.MemberInit(
          newExpression: Expression.New(
            entity.Initializer.Constructor,
            arguments: entity.Initializer.ConstructorProperties.Select(column => MakeGetValueCallExpression(column.Type, column.Index))),
          bindings: entity.Initializer.InitializerProperties.Select(column => Expression.Bind(
            member: column.Member,
            expression: MakeGetValueCallExpression(column.Type, column.Index)))),
        parameters: new[] { readerParam, offsetParam });

      return readerExpr.Compile();
    }

    Expression MakeGetValueCallExpression(Type type, int index)
    {
      MethodInfo readMethod = GetReadMethod(type);
      Expression indexExpr = Expression.Add(
        left: offsetParam,
        right: Expression.Constant(index));

      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
        ? Expression.Call(
            method: readMethod,
            arg0: readerParam,
            arg1: indexExpr)
        : Expression.Call(
            instance: readerParam,
            method: readMethod,
            arguments: indexExpr);
    }

    static MethodInfo GetReadMethod(Type type)
    {
      if (type.IsEnum)
        return GetReadMethod(type.GetEnumUnderlyingType());

      if (s_getValueMethods.TryGetValue(type, out MethodInfo? method))
        return method;

      throw new NotSupportedException($"Property type '{type.Name}' is not supported.");
    }
  }

  public static RowReaderProvider Create()
  {
    return new(new ConcurrentDictionary<IEntityModel, RowReader>());
  }

  [Conditional("DEBUG")]
  private static void EnsureCanReadSupportedTypes(IReadOnlyDictionary<Type, MethodInfo> getValueMethods)
  {
    foreach (Type type in Configuration.SupportedColumnTypes)
    {
      Debug.Assert(getValueMethods.ContainsKey(type), $"Cannot read supported type '{type.Name}'.");
    }
  }
}
