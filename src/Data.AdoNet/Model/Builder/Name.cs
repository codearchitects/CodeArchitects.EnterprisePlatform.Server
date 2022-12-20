namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

/// <summary>
/// Represents either a member name or a column name.
/// </summary>
public readonly struct Name
{
  private readonly string _memberName;
  private readonly ColumnName _columnName;

  /// <summary>
  /// Creates a new <see cref="Name"/> representing a member name.
  /// </summary>
  /// <param name="memberName">The name of the member.</param>
  public Name(string memberName)
  {
    _memberName = memberName;
    _columnName = default;
  }

  /// <summary>
  /// Creates a new <see cref="Name"/> representing a column name.
  /// </summary>
  /// <param name="columnName">The name of the column.</param>
  public Name(ColumnName columnName)
  {
    _memberName = string.Empty;
    _columnName = columnName;
  }

  /// <summary>
  /// <c>true</c> if the instance is representing a column name, <c>false</c> if it is representing a member name.
  /// </summary>
  public bool IsColumnName => _memberName == string.Empty;

  /// <summary>
  /// The raw string value of the name.
  /// </summary>
  public string Value => IsColumnName ? _columnName.Name : _memberName;

  /// <summary>
  /// Creates a new <see cref="Name"/> representing a member name.
  /// </summary>
  /// <param name="memberName">The name of the member.</param>
  public static implicit operator Name(string memberName) => new(memberName);

  /// <summary>
  /// Creates a new <see cref="Name"/> representing a column name.
  /// </summary>
  /// <param name="columnName">The name of the column.</param>
  public static implicit operator Name(ColumnName columnName) => new(columnName);
}
