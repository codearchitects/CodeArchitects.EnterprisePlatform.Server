using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document.Expressions;

internal class ObjectValueNode : ListIterator<IField, IObjectFieldNode>, IObjectValueNode
{
  private readonly INodeContext _context;
  private readonly IObjectType _type;
  private readonly object _object;

  public ObjectValueNode(INodeContext context, IObjectType type, object @object)
  {
    _context = context;
    _type = type;
    _object = @object;
  }

  public IEnumerable<IObjectFieldNode> Fields => this;

  protected override IReadOnlyList<IField> List => _type.Fields;

  protected override IObjectFieldNode OnCurrent(IField field)
  {
    object? value = field.GetValue(_object);

    return new ObjectFieldNode(field.Name, NodeFactory.CreateValue(_context, value));
  }
}
