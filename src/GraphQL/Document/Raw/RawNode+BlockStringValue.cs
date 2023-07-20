using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document.Raw;

internal partial class RawNode : IBlockStringValueNode
{
  IEnumerable<string> IBlockStringValueNode.Lines => throw new NotImplementedException();
}
