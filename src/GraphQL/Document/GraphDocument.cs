using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Diagnostics;

namespace CodeArchitects.Platform.GraphQL.Document;

[DebuggerDisplay("{ToString()}")]
public abstract class GraphDocument
{
  private protected GraphDocument() { }

  public abstract OperationType OperationType { get; }

  public abstract string Name { get; }

  public abstract IOperationDefinitionNode CreateOperationDefinition(IGraphDocumentContext context);

  /// <inheritdoc/>
  public override string ToString()
  {
    OperationType operationType = OperationType;
    string name = Name;

    if (name is "")
    {
      name = "<unnamed>";
    }

    return $"{operationType} {name}";
  }
}

public abstract class GraphDocument<TResult> : GraphDocument
  where TResult : class
{
}

public abstract class GraphDocument<TResult, TVariables> : GraphDocument
  where TResult : class
  where TVariables : notnull
{
  public override IOperationDefinitionNode CreateOperationDefinition(IGraphDocumentContext context)
  {
    IReadOnlyList<IVariable> variables = context.Model.GetVariables(typeof(TVariables));
    return CreateOperationDefinition(context, variables);
  }

  protected abstract IOperationDefinitionNode CreateOperationDefinition(IGraphDocumentContext context, IReadOnlyList<IVariable> variables);
}
