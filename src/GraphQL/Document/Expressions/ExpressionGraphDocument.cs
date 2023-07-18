using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class ExpressionGraphDocument<TResult> : GraphDocument<TResult>
  where TResult : class
{
  private readonly Expression _expression;

  private ExpressionGraphDocument(string name, Expression expression)
  {
    Name = name;
    _expression = expression;
  }

  public sealed override string Name { get; }

  public sealed class Query : ExpressionGraphDocument<TResult>
  {
    public Query(string name, Expression expression)
      : base(name, expression)
    {
    }

    public override OperationType OperationType => OperationType.Query;

    public override IOperationDefinitionNode CreateOperationDefinition(IGraphDocumentContext context)
      => new QueryDefinitionNode(context.GetService<INodeContext>(), Name, Array.Empty<IVariable>(), _expression);
  }

  public sealed class Mutation : ExpressionGraphDocument<TResult>
  {
    public Mutation(string name, Expression expression)
      : base(name, expression)
    {
    }

    public override OperationType OperationType => OperationType.Mutation;

    public override IOperationDefinitionNode CreateOperationDefinition(IGraphDocumentContext context)
      => new MutationDefinitionNode(context.GetService<INodeContext>(), Name, Array.Empty<IVariable>(), _expression);
  }
}

internal abstract class ExpressionGraphDocument<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TResult : class
  where TVariables : notnull
{
  private readonly Expression _expression;

  private ExpressionGraphDocument(string name, Expression expression)
  {
    Name = name;
    _expression = expression;
  }

  public sealed override string Name { get; }

  public sealed class Query : ExpressionGraphDocument<TResult, TVariables>
  {
    public Query(string name, Expression expression)
      : base(name, expression)
    {
    }

    public override OperationType OperationType => OperationType.Query;

    protected override IOperationDefinitionNode CreateOperationDefinition(IGraphDocumentContext context, IReadOnlyList<IVariable> variables)
      => new QueryDefinitionNode(context.GetService<INodeContext>(), Name, variables, _expression);
  }

  public sealed class Mutation : ExpressionGraphDocument<TResult, TVariables>
  {
    public Mutation(string name, Expression expression)
      : base(name, expression)
    {
    }

    public override OperationType OperationType => OperationType.Mutation;

    protected override IOperationDefinitionNode CreateOperationDefinition(IGraphDocumentContext context, IReadOnlyList<IVariable> variables)
      => new MutationDefinitionNode(context.GetService<INodeContext>(), Name, variables, _expression);
  }
}
