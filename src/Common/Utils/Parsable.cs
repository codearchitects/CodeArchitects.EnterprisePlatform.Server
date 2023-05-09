// This enable using Type.Parse(string) without static abstract interface members from .NET 7

namespace CodeArchitects.Platform.Common.Utils;

using System.Reflection;

#if NET7_0_OR_GREATER

internal static class Parsable<TParsable>
  where TParsable : IParsable<TParsable>
{
  public static readonly Parse<TParsable> Parse;

  static Parsable()
  {
    Parse = s => TParsable.Parse(s, null);
  }

  public static void EnsureInitialized()
  {
    // This just triggers the static constructor
  }
}

#else

using CodeArchitects.Platform.Common.Exceptions;
using System.Linq.Expressions;

internal static class Parsable<TParsable>
{
  public static readonly Parse<TParsable> Parse;

  static Parsable()
  {
    MethodInfo? parseMethod = typeof(TParsable).GetMethod(
      name: "Parse",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      binder: null,
      types: new[] { typeof(string) },
      modifiers: null);

    if (parseMethod is not null)
    {
      Parse = (Parse<TParsable>)parseMethod.CreateDelegate(typeof(Parse<TParsable>));
      return;
    }

    parseMethod = typeof(TParsable).GetMethod(
      name: "Parse",
      bindingAttr: BindingFlags.Static | BindingFlags.Public,
      binder: null,
      types: new[] { typeof(string), typeof(IFormatProvider) },
      modifiers: null);

    if (parseMethod is not null)
    {
      ParameterExpression sParam = Expression.Parameter(typeof(string), "s");
      Expression<Parse<TParsable>> parseExpr = Expression.Lambda<Parse<TParsable>>(
        body: Expression.Call(
          method: parseMethod,
          arg0: sParam,
          arg1: Expression.Constant(null, typeof(IFormatProvider))),
        parameters: sParam);

      Parse = parseExpr.Compile();
    }

    throw new TypeArgumentException($"'{typeof(TParsable).Name}' does not define a public static method Parse(string) or Parse(string, IFormatProvider).", nameof(TParsable));
  }

  public static void EnsureInitialized()
  {
    // This just triggers the static constructor
  }
}

#endif

internal static class Parsable
{
  public static void EnsureInitialized(Type parsableType)
  {
    typeof(Parsable<>)
      .MakeGenericType(parsableType)
      .GetRequiredMethod(
        name: nameof(Parsable<int>.EnsureInitialized),
        bindingAttr: BindingFlags.Static | BindingFlags.Public,
        types: Type.EmptyTypes)
      .Invoke(null, Array.Empty<object?>());
  }
}