namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

/// <summary>
/// Represents the name of a database colimn.
/// </summary>
public readonly struct ColumnName
{
  /// <summary>
  /// Creates a new <see cref="ColumnName"/> with the provided name.
  /// </summary>
  /// <param name="name">The name of the column.</param>
  public ColumnName(string name)
  {
    Name = name;
  }

  /// <summary>
  /// The name of the column.
  /// </summary>
  public string Name { get; }
}
