using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ObjectValueNode : ListIterator<IField, IObjectFieldNode>, IObjectValueNode
{
  private readonly INodeRoot _root;
  private readonly IObjectType _type;
  private readonly object _object;

  public ObjectValueNode(INodeRoot root, IObjectType type, object @object)
  {
    _root = root;
    _type = type;
    _object = @object;
  }

  public ValueNodeKind ValueKind => ValueNodeKind.ObjectValue;

  public IEnumerable<IObjectFieldNode> Fields => this;

  protected override IReadOnlyList<IField> List => _type.Fields;

  protected override IObjectFieldNode OnCurrent(IField field)
  {
    object? value = field.GetValue(_object);

    return new ObjectFieldNode(_root, field.Name, value);
  }
}
