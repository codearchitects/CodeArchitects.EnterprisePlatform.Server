using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal partial class DocumentBuilder<TDocumentRoot, TSymbol>
{
  public GraphDocument<TResult> Mutation<TResult>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildMutation(null, expansion);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildMutation(null, expansion);
  }

  public GraphDocument<TResult> Mutation<TResult>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TResult>>> expansion)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildMutation(name, expansion);
  }

  public GraphDocument<TResult, TVariables> Mutation<TResult, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TResult, TVariables>>> expansion)
    where TVariables : notnull
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildMutation(name, expansion);
  }
}
