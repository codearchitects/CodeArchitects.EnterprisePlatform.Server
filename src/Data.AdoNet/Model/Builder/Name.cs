namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

public readonly struct Name
{
  private readonly string _memberName;
  private readonly ColumnName _columnName;

  public Name(string memberName)
  {
    _memberName = memberName;
    _columnName = default;
  }

  public Name(ColumnName columnName)
  {
    _memberName = string.Empty;
    _columnName = columnName;
  }

  public bool IsColumnName => _columnName.Equals(default);

  public string Value => IsColumnName ? _memberName : _columnName.Name;

  public static implicit operator Name(string memberName) => new(memberName);

  public static implicit operator Name(ColumnName columnName) => new(columnName);
}
