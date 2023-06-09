namespace CodeArchitects.Platform.GraphQL.Document;

internal class DocumentBuilderOptions
{
  public ValueSeparator Separator { get; set; } = ValueSeparator.Comma;
  public LinePolicy LinePolicy { get; set; } = LinePolicy.Default;
}
