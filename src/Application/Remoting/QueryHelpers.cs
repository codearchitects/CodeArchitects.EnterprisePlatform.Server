using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

namespace CodeArchitects.Platform.Application.Remoting;

/// <summary>
/// Helper methods for constructing http query strings.
/// </summary>
public static class QueryHelpers
{
  private delegate void QueryDelegate(StringBuilder url, object? queryObject);

  private static readonly ConcurrentDictionary<Type, QueryDelegate> s_queryDelegates = new();
  
  private static readonly MethodInfo s_appendSingleMethod = typeof(QueryHelpers).GetRequiredMethod(
    name: nameof(AppendSingle),
    bindingAttr: BindingFlags.NonPublic | BindingFlags.Static,
    types: new[] { typeof(StringBuilder), typeof(string), typeof(object) });
  
  private static readonly MethodInfo s_appendManyMethod = typeof(QueryHelpers).GetRequiredMethod(
    name: nameof(AppendMany),
    bindingAttr: BindingFlags.NonPublic | BindingFlags.Static,
    types: new[] { typeof(StringBuilder), typeof(string), typeof(IEnumerable<object>) });

  private static readonly MethodInfo s_appendChar = typeof(StringBuilder).GetRequiredMethod(
    name: nameof(StringBuilder.Append),
    bindingAttr: BindingFlags.Public | BindingFlags.Instance,
    types: new[] { typeof(char) });

  /// <summary>
  /// Encodes an object as query string and appends it to a given url.
  /// </summary>
  /// <typeparam name="T">The type of the object.</typeparam>
  /// <param name="url">The base url.</param>
  /// <param name="queryObject">The object to encode.</param>
  /// <returns>The combined result.</returns>
  public static string AddQueryToUrl<T>(string url, T? queryObject)
    where T : class
  {
    QueryDelegate @delegate = s_queryDelegates.GetOrAdd(typeof(T), CreateDelegate);
    StringBuilder sb = new(url);
    sb.Append('?');

    @delegate(sb, queryObject);

    return sb.ToString();
  }

  /// <summary>
  /// Encodes two objects as query string and appends it to a given url.
  /// </summary>
  /// <typeparam name="T1">The type of the first object.</typeparam>
  /// <typeparam name="T2">The type of the second object.</typeparam>
  /// <param name="url">The base url.</param>
  /// <param name="queryObject1">The first object to encode.</param>
  /// <param name="queryObject2">The second object to encode.</param>
  /// <returns>The combined result.</returns>
  public static string AddQueryToUrl<T1, T2>(string url, T1? queryObject1, T2? queryObject2)
    where T1 : class
    where T2 : class
  {
    QueryDelegate @delegate;
    StringBuilder sb = new(url);
    sb.Append('?');

    @delegate = s_queryDelegates.GetOrAdd(typeof(T1), CreateDelegate);
    @delegate(sb, queryObject1);
    sb.Append('&');

    @delegate = s_queryDelegates.GetOrAdd(typeof(T2), CreateDelegate);
    @delegate(sb, queryObject2);

    return sb.ToString();
  }

  /// <summary>
  /// Encodes three objects as query string and appends it to a given url.
  /// </summary>
  /// <typeparam name="T1">The type of the first object.</typeparam>
  /// <typeparam name="T2">The type of the second object.</typeparam>
  /// <typeparam name="T3">The type of the third object.</typeparam>
  /// <param name="url">The base url.</param>
  /// <param name="queryObject1">The first object to encode.</param>
  /// <param name="queryObject2">The second object to encode.</param>
  /// <param name="queryObject3">The third object to encode.</param>
  /// <returns>The combined result.</returns>
  public static string AddQueryToUrl<T1, T2, T3>(string url, T1 queryObject1, T2 queryObject2, T3 queryObject3)
    where T1 : class
    where T2 : class
    where T3 : class
  {
    QueryDelegate @delegate;
    StringBuilder sb = new(url);
    sb.Append('?');

    @delegate = s_queryDelegates.GetOrAdd(typeof(T1), CreateDelegate);
    @delegate(sb, queryObject1);
    sb.Append('&');

    @delegate = s_queryDelegates.GetOrAdd(typeof(T2), CreateDelegate);
    @delegate(sb, queryObject2);
    sb.Append('&');

    @delegate = s_queryDelegates.GetOrAdd(typeof(T3), CreateDelegate);
    @delegate(sb, queryObject3);

    return sb.ToString();
  }

  /// <summary>
  /// Encodes multiple objects as query string and appends it to a given url.
  /// </summary>
  /// <param name="url">The base url.</param>
  /// <param name="queryObjects">The objects to encode.</param>
  /// <returns>The combined result.</returns>
  public static string AddQueryToUrl(string url, params object?[] queryObjects)
  {
    StringBuilder sb = new(url);
    sb.Append('?');

    for (int i = 0; i < queryObjects.Length; i++)
    {
      object? queryObject = queryObjects[i];
      if (queryObject is null)
        continue;

      QueryDelegate @delegate = s_queryDelegates.GetOrAdd(queryObject.GetType(), CreateDelegate);

      @delegate(sb, queryObject);
      if (i != queryObjects.Length - 1)
      {
        sb.Append('&');
      }
    }

    return sb.ToString();
  }

  private static QueryDelegate CreateDelegate(Type type)
  {
    ParameterExpression url = Expression.Parameter(typeof(StringBuilder), nameof(url));
    ParameterExpression queryObject = Expression.Parameter(typeof(object), nameof(queryObject));

    List<Expression> statements = new();
    AddToStatements(type, url, Expression.Convert(queryObject, type), null, statements);

    LambdaExpression expression = Expression.Lambda(
      delegateType: typeof(QueryDelegate),
      body: Expression.Block(statements),
      parameters: new[] { url, queryObject });

    return (QueryDelegate)expression.Compile();
  }

  private static void AddToStatements(Type type, Expression url, Expression queryObject, string? path, ICollection<Expression> statements)
  {
    PropertyInfo[] properties = type.GetProperties();

    for (int i = 0; i < properties.Length; i++)
    {
      PropertyInfo property = properties[i];
      Type propertyType = property.PropertyType;

      string name = path is null
        ? property.Name
        : $"{path}.{property.Name}";

      if (IsSimpleType(propertyType))
      {
        Expression statement;
        if (typeof(IEnumerable<object>).IsAssignableFrom(propertyType))
        {
          statement = Expression.Call(
            instance: null,
            method: s_appendManyMethod,
            arg0: url,
            arg1: Expression.Constant(name),
            arg2: Expression.Convert(
              expression: Expression.Property(queryObject, property),
              type: typeof(IEnumerable<object>)));
        }
        else
        {
          statement = Expression.Call(
            instance: null,
            method: s_appendSingleMethod,
            arg0: url,
            arg1: Expression.Constant(name),
            arg2: Expression.Convert(
              expression: Expression.Property(queryObject, property),
              type: typeof(object)));
        }
        statements.Add(statement);
      }
      else
      {
        AddToStatements(propertyType, url, Expression.Property(queryObject, property), name, statements);
      }
      if (i != properties.Length - 1)
      {
        statements.Add(Expression.Call(
          instance: url,
          method: s_appendChar,
          arguments: Expression.Constant('&')));
      }
    }
  }

  private static void AppendSingle(StringBuilder sb, string name, object? obj)
  {
    if (obj is null)
      return;

    sb.Append(HttpUtility.UrlEncode(name, Encoding.UTF8));
    sb.Append('=');
    sb.Append(HttpUtility.UrlEncode(obj.ToString(), Encoding.UTF8));
  }

  private static void AppendMany(StringBuilder sb, string name, IEnumerable<object?> objs)
  {
    IEnumerator<object?> enumerator = objs.GetEnumerator();

    object? obj;
    if (enumerator.MoveNext())
    {
      obj = enumerator.Current;
      AppendSingle(sb, name, obj);
    }

    while (enumerator.MoveNext())
    {
      obj = enumerator.Current;
      if (obj is null)
        continue;

      sb.Append('&');
      AppendSingle(sb, name, obj);
    }
  }

  internal static bool IsSimpleType(Type type)
  {
    return IsSimpleType(type, 0);
  }

  private static bool IsSimpleType(Type type, int nesting)
  {
    bool isSimpleType =
      type.IsPrimitive         ||
      type == typeof(string)   ||
      type == typeof(decimal)  ||
      type == typeof(Guid)     ||
      type == typeof(DateTime) ||
      type == typeof(DateTimeOffset);

    if (isSimpleType)
      return true;

    if (IsEnumerable(type, true, out Type? elementType))
    {
      if (nesting > 0)
        return false;

      return IsSimpleType(elementType, nesting + 1);
    }

    return false;
  }

  private static bool IsEnumerable(Type type, bool checkInterfaces, [NotNullWhen(true)] out Type? elementType)
  {
    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
    {
      elementType = type.GetGenericArguments()[0];
      return true;
    }

    if (!checkInterfaces)
    {
      elementType = null;
      return false;
    }

    foreach (Type @interface in type.GetInterfaces())
    {
      if (IsEnumerable(@interface, false, out elementType))
      {
        return true;
      }
    }

    elementType = null;
    return false;
  }
}
