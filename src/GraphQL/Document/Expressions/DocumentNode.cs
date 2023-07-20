using CodeArchitects.Platform.Common.Exceptions;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal abstract class DocumentNode : INodeRoot, IDocumentNode, IEnumerable<IDefinitionNode>, IEnumerator<IDefinitionNode>
{
  private readonly INodeContext _context;
  private readonly string _name;
  private readonly IReadOnlyList<IVariable> _variables;
  private readonly Expression _expression;
  private readonly HashSet<GraphFragment> _fragments;
  private HashSet<GraphFragment>.Enumerator _fragmentsEnumerator;
  private int _state;

  public DocumentNode(INodeContext context, string name, IReadOnlyList<IVariable> variables, Expression expression)
  {
    _context = context;
    _name = name;
    _variables = variables;
    _expression = expression;
    _fragments = new(GraphFragmentEqualityComparer.Instance);
  }

  INodeContext INodeRoot.Context => _context;

  public IEnumerable<IDefinitionNode> Definitions => this;

  IDefinitionNode IEnumerator<IDefinitionNode>.Current
  {
    get
    {
      switch (_state)
      {
        case 1:
          return CreateOperationDefinition(_name, _variables, _expression);

        case 2:
          return _fragmentsEnumerator.Current.CreateFragmentDefinition(_context);

        default:
          Debug.Fail("Invalid iterator state.");
          throw Errors.Unreachable;
      }
    }
  }

  protected abstract OperationDefinitionNode CreateOperationDefinition(string name, IReadOnlyList<IVariable> variables, Expression expression);

  IEnumerator<IDefinitionNode> IEnumerable<IDefinitionNode>.GetEnumerator() => this;

  bool IEnumerator.MoveNext()
  {
    switch (_state)
    {
      case 0:
        _state = 1;
        return true;

      case 1:
        _state = 2;
        _fragmentsEnumerator = _fragments.GetEnumerator();
        return _fragmentsEnumerator.MoveNext();

      case 2:
        return _fragmentsEnumerator.MoveNext();

      default:
        Debug.Fail("Invalid iterator state.");
        return false;
    }
  }

  void INodeRoot.ReportFragment(GraphFragment fragment)
  {
    if (!_fragments.Add(fragment) && _fragments.TryGetValue(fragment, out GraphFragment? existingFragment))
    {
      if (fragment == existingFragment)
        return;

      throw new InvalidOperationException("Two different fragments with the same name were used inside the same document.");
    }
  }

  private sealed class GraphFragmentEqualityComparer : EqualityComparer<GraphFragment>
  {
    public static readonly GraphFragmentEqualityComparer Instance = new();

    private GraphFragmentEqualityComparer() { }

    public override bool Equals(GraphFragment x, GraphFragment y) => x.Name == y.Name;

    public override int GetHashCode(GraphFragment obj) => obj.Name.GetHashCode();
  }

  #region Not relevant

  [ExcludeFromCodeCoverage]
  object IEnumerator.Current => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  void IDisposable.Dispose() { }

  [ExcludeFromCodeCoverage]
  void IEnumerator.Reset() => throw new NotSupportedException();

  #endregion
}
