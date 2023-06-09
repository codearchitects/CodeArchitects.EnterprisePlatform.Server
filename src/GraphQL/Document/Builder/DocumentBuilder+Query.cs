using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Builder;

internal partial class DocumentBuilder<TDocumentRoot>
{
  public GraphDocument<TField> Query<TField>(Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion)
  {
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildQuery(null, expansion);
  }

  public GraphDocument<TField, TVariables> Query<TField, TVariables>(Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull
  {
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildQuery(null, expansion);
  }

  public GraphDocument<TField> Query<TField>(string name, Expression<Func<IOperationBuilder<TDocumentRoot>, IBuildResult<TField>>> expansion)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildQuery(name, expansion);
  }

  public GraphDocument<TField, TVariables> Query<TField, TVariables>(string name, Expression<Func<IOperationBuilder<TDocumentRoot, TVariables>, IBuildResult<TField, TVariables>>> expansion)
    where TVariables : notnull
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
    if (expansion is null)
      throw new ArgumentNullException(nameof(expansion));

    return BuildQuery(name, expansion);
  }
}
