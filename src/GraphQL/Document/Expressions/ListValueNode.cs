using CodeArchitects.Platform.GraphQL.Document.Nodes;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ListValueNode : IListValueNode, IEnumerable<IValueNode>, IEnumerator<IValueNode>
{
  private readonly INodeRoot _root;
  private readonly IEnumerator _values;

  public ListValueNode(INodeRoot root, IEnumerator values)
  {
    _root = root;
    _values = values;
  }

  public ValueNodeKind ValueKind => ValueNodeKind.ListValue;

  public IEnumerable<IValueNode> Values => this;

  IValueNode IEnumerator<IValueNode>.Current => NodeFactory.CreateValue(_root, _values.Current);

  IEnumerator<IValueNode> IEnumerable<IValueNode>.GetEnumerator() => this;

  bool IEnumerator.MoveNext() => _values.MoveNext();

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
