namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal static class MethodNames
{
  public const string WithArgument       = nameof(WithArgument);
  public const string WithDirective      = nameof(WithDirective);
  public const string WithSelection      = nameof(WithSelection);
  public const string WithSelectionSet   = nameof(WithSelectionSet);
  public const string WithInlineFragment = nameof(WithInlineFragment);
  public const string WithFragmentSpread = nameof(WithFragmentSpread);
  public const string Expand             = nameof(Expand);
  public const string Select             = nameof(Select);
  public const string Field              = nameof(Field);

  public static readonly Predicate<string> IsArgument       = methodName => methodName is WithArgument;
  public static readonly Predicate<string> IsDirective      = methodName => methodName is WithDirective;
  public static readonly Predicate<string> IsSelectionSet   = methodName => methodName is WithSelection or WithSelectionSet;
  public static readonly Predicate<string> IsFragmentSpread = methodName => methodName is WithFragmentSpread;
}
