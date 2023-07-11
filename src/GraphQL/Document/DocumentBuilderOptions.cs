namespace CodeArchitects.Platform.GraphQL.Document;

public class DocumentBuilderOptions
{
  public ValueSeparator Separator { get; set; } = ValueSeparator.Comma;
  public LinePolicy LinePolicy { get; set; } = LinePolicy.Default;
}
