using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CodeArchitects.Platform.GraphQL.Document;
using CodeArchitects.Platform.GraphQL.Document.Builder;
using CodeArchitects.Platform.GraphQL.Document.Nodes;
using CodeArchitects.Platform.GraphQL.Fixtures.Model;
using CodeArchitects.Platform.GraphQL.Model;
using Microsoft.IO;
using Moq;
using System.Linq.Expressions;
using System.Text;

BenchmarkRunner.Run<Benchmarks>();

[MemoryDiagnoser]
public class Benchmarks
{
  private static readonly Mock<INodeContext> s_nodeContextMock = CreateNodeContextMock();
  private static readonly Mock<IModel> s_modelMock = CreateModelMock();
  private static readonly RecyclableMemoryStreamManager s_msManager = new(1024 * 64, 1024 * 1024 * 64);
  private static readonly MyDocumentBuilder<IDocumentRoot> s_sut = new(s_nodeContextMock.Object, s_modelMock.Object, new(), s_msManager);

  private static Mock<IModel> CreateModelMock()
  {
    var result = new Mock<IModel>(MockBehavior.Strict);

    return result;
  }

  private static Mock<INodeContext> CreateNodeContextMock()
  {
    var result = new Mock<INodeContext>(MockBehavior.Strict);

    LambdaExpression? defaultSelection = null;
    result
      .Setup(x => x.TryGetDefaultSelection(typeof(string), out defaultSelection))
      .Returns(false);

    return result;
  }

  [Benchmark]
  public object BuildQuery()
  {
    return s_sut.Query("GetBlogs", _ => _
      .WithDirective("include", _ => _
        .WithArgument("all", true))
      .WithSelection(root => new GetBlogsResult
      {
        Blogs = _.SelectRef(root.Field<Connection<Blog>>("blogs"), connection => new Connection<Blog>
        {
          Edges = _.SelectCol(connection.Edges, edge => new Edge<Blog>
          {
            Cursor = edge.Cursor
          })
        })
      }));
  }
}

class MyDocumentBuilder<TDocumentRoot> : DocumentBuilder<TDocumentRoot>
  where TDocumentRoot : notnull
{
  public MyDocumentBuilder(INodeContext nodeContext, IModel model, DocumentBuilderOptions options, RecyclableMemoryStreamManager msManager)
    : base(nodeContext, model, options, msManager)
  {
  }

  protected override GraphDocument<TResult, TVariables> CreateDocument<TResult, TVariables>(OperationType type, string? name, byte[] content)
  {
    return new MyGraphDocument<TResult, TVariables>(content);
  }
}

class MyGraphDocument<TResult, TVariables> : GraphDocument<TResult, TVariables>
  where TVariables : notnull
{
  public MyGraphDocument(byte[] content)
  {
    Content = content;
  }

  public byte[] Content { get; }

  public override string ToString() => Encoding.UTF8.GetString(Content);
}
