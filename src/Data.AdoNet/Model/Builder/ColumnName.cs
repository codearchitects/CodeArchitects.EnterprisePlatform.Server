namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

public readonly struct ColumnName
{
  public ColumnName(string name)
  {
    Name = name;
  }

  public string Name { get; }
}
