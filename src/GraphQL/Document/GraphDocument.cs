using CodeArchitects.Platform.GraphQL.Document.Expressions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document;

public abstract class GraphDocument
{
  private readonly Expression _expression;

  private protected GraphDocument(string? name, Expression expression)
  {
    Name = name;
    _expression = expression;
  }

  public abstract OperationType OperationType { get; }

  public string? Name { get; }

  internal abstract IOperationDefinitionNode CreateOperationDefinition(IModel model, INodeContext nodeContext);

  private protected QueryDefinitionNode CreateQueryDefinition(INodeContext nodeContext, IEnumerable<IVariable> variables)
  {
    return new QueryDefinitionNode(nodeContext, Name, variables, _expression);
  }

  private protected MutationDefinitionNode CreateMutationDefinition(INodeContext nodeContext, IEnumerable<IVariable> variables)
  {
    return new MutationDefinitionNode(nodeContext, Name, variables, _expression);
  }

  /// <inheritdoc/>
  public override string ToString() => $"{OperationType} {Name ?? "<unnamed>"}";
}

public abstract class GraphDocument<TResult> : GraphDocument
{
  internal GraphDocument(string? name, Expression expression)
    : base(name, expression)
  {
  }

  private protected abstract IOperationDefinitionNode CreateOperationDefinition(INodeContext nodeContext);

  internal sealed override IOperationDefinitionNode CreateOperationDefinition(IModel model, INodeContext nodeContext)
  {
    return CreateOperationDefinition(nodeContext);
  }
}

public abstract class GraphDocument<TResult, TVariables> : GraphDocument
  where TVariables : notnull
{
  internal GraphDocument(string? name, Expression expression)
    : base(name, expression)
  {
  }

  private protected abstract IOperationDefinitionNode CreateOperationDefinition(INodeContext nodeContext, IEnumerable<IVariable> variables);

  internal sealed override IOperationDefinitionNode CreateOperationDefinition(IModel model, INodeContext nodeContext)
  {
    IReadOnlyCollection<IVariable> variables = model.GetVariables(typeof(TVariables));

    return CreateOperationDefinition(nodeContext, variables);
  }
}

internal sealed class GraphQuery<TResult> : GraphDocument<TResult>
{
  public GraphQuery(string? name, Expression expression)
    : base(name, expression)
  {
  }

  public override OperationType OperationType => OperationType.Query;

  private protected override IOperationDefinitionNode CreateOperationDefinition(INodeContext nodeContext)
    => CreateQueryDefinition(nodeContext, Enumerable.Empty<IVariable>());
}

internal sealed class GraphMutation<TResult> : GraphDocument<TResult>
{
  public GraphMutation(string? name, Expression expression)
    : base(name, expression)
  {
  }

  public override OperationType OperationType => OperationType.Mutation;

  private protected override IOperationDefinitionNode CreateOperationDefinition(INodeContext nodeContext)
    => CreateMutationDefinition(nodeContext, Enumerable.Empty<IVariable>());
}

internal sealed class GraphQuery<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  public GraphQuery(string? name, Expression expression)
    : base(name, expression)
  {
  }

  public override OperationType OperationType => OperationType.Query;

  private protected override IOperationDefinitionNode CreateOperationDefinition(INodeContext nodeContext, IEnumerable<IVariable> variables)
    => CreateQueryDefinition(nodeContext, variables);
}

internal sealed class GraphMutation<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  public GraphMutation(string? name, Expression expression)
    : base(name, expression)
  {
  }

  public override OperationType OperationType => OperationType.Mutation;

  private protected override IOperationDefinitionNode CreateOperationDefinition(INodeContext nodeContext, IEnumerable<IVariable> variables)
    => CreateMutationDefinition(nodeContext, variables);
}
