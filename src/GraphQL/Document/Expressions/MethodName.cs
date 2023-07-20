namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal static class MethodName
{
  public const string WithArgument       = nameof(WithArgument);
  public const string WithDirective      = nameof(WithDirective);
  public const string WithSelection      = nameof(WithSelection);
  public const string WithSelectionSet   = nameof(WithSelectionSet);
  public const string WithInlineFragment = nameof(WithInlineFragment);
  public const string WithFragmentSpread = nameof(WithFragmentSpread);
  public const string ExpandRef          = nameof(ExpandRef);
  public const string ExpandCol          = nameof(ExpandCol);
  public const string SelectRef          = nameof(SelectRef);
  public const string SelectCol          = nameof(SelectCol);
  public const string IncludeRef         = nameof(IncludeRef);
  public const string IncludeCol         = nameof(IncludeCol);
  public const string Field              = nameof(Field);

  public static class Represents
  {
    public static readonly Predicate<string> Argument       = methodName => methodName is WithArgument;
    public static readonly Predicate<string> Directive      = methodName => methodName is WithDirective;
    public static readonly Predicate<string> SelectionSet   = methodName => methodName is WithSelection or WithSelectionSet;
    public static readonly Predicate<string> FragmentSpread = methodName => methodName is WithFragmentSpread;
  }
}
