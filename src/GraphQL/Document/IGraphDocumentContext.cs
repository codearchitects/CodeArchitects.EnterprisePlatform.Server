using CodeArchitects.Platform.GraphQL.Model;

namespace CodeArchitects.Platform.GraphQL.Document;

public interface IGraphDocumentContext
{
  IModel Model { get; }

  TService GetService<TService>();
}
