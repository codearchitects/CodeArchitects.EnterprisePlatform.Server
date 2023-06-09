using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CodeArchitects.Platform.GraphQL.Document.Builder.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Builder.Expressions;

internal class ListValueNode : IListValueNode, IEnumerable<object?>, IEnumerator<object?>
{
  private readonly INodeContext _context;
  private readonly IEnumerator _values;

  public ListValueNode(INodeContext context, IEnumerator values)
  {
    _context = context;
    _values = values;
  }

  public IEnumerable<object?> Values => this;

  object? IEnumerator<object?>.Current => NodeFactory.CreateValue(_context, _values.Current);

  IEnumerator<object?> IEnumerable<object?>.GetEnumerator() => this;

  bool IEnumerator.MoveNext() => _values.MoveNext();

  void IDisposable.Dispose()
  {
  }

  #region Not relevant

  [ExcludeFromCodeCoverage]
  object IEnumerator.Current => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

  [ExcludeFromCodeCoverage]
  void IEnumerator.Reset() => throw new NotSupportedException();

  #endregion
}
