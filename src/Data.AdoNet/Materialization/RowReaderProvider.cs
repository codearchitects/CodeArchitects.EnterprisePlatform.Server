using CodeArchitects.Platform.Data.AdoNet.Model;
using System.Collections.Concurrent;
using System.Data;
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

    return new RowReader(keyReader, entityReader, entity, entity.PrimaryKey.Properties[0].Index);

    KeyReader CreateSimpleKeyReader()
    {
      Expression<KeyReader> keyReaderExpr = Expression.Lambda<KeyReader>(
        body: MakeGetValueCallExpression(entity.PrimaryKey.Type),
        parameters: new[] { readerParam, offsetParam });

      return keyReaderExpr.Compile();
    }

    KeyReader CreateCompositeKeyReader()
    {
      Expression<KeyReader> keyReaderExpr = Expression.Lambda<KeyReader>(
        body: Expression.New(
          constructor: entity.PrimaryKey.TupleConstructor,
          arguments: entity.PrimaryKey.Properties.Select(property => MakeGetValueCallExpression(property.Type))),
        parameters: new[] { readerParam, offsetParam });

      return keyReaderExpr.Compile();
    }

    EntityReader CreateEntityReader()
    {
      Expression<EntityReader> readerExpr = Expression.Lambda<EntityReader>(
        body: Expression.MemberInit(
          newExpression: Expression.New(
            entity.Initializer.Constructor,
            arguments: entity.Initializer.ConstructorProperties.Select(property => MakeGetValueCallExpression(property.Type))),
          bindings: entity.Initializer.InitializerProperties.Select(property => Expression.Bind(
            member: property.Member,
            expression: MakeGetValueCallExpression(property.Type)))),
        parameters: new[] { readerParam, offsetParam });

      return readerExpr.Compile();
    }

    Expression MakeGetValueCallExpression(Type type)
    {
      MethodInfo readMethod = GetReadMethod(type);

      return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)
        ? Expression.Call(
            method: readMethod,
            arg0: readerParam,
            arg1: offsetParam)
        : Expression.Call(
            instance: readerParam,
            method: readMethod,
            arguments: offsetParam);
    }

    static MethodInfo GetReadMethod(Type type)
    {
      if (type.IsEnum)
        return GetReadMethod(type.GetEnumUnderlyingType());

      if (s_getValueMethods.TryGetValue(type, out MethodInfo method))
        return method;

      throw new NotSupportedException($"Property type '{type.Name}' is not supported.");
    }
  }

  public static RowReaderProvider Create()
  {
    return new(new ConcurrentDictionary<IEntityModel, RowReader>());
  }
}
