using CodeArchitects.Platform.GraphQL.Document.Nodes;

namespace CodeArchitects.Platform.GraphQL.Document;

internal interface IDocumentCompiler<TUtf8Document>
  where TUtf8Document : IUtf8Document
{
  TUtf8Document Compile(OperationType operationType, string name, IOperationDefinitionNode operationDefinition);
}