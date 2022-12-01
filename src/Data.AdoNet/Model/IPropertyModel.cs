namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IPropertyModel : IPropertyModelBase
{
  bool IsPrimaryKey { get; }

  string ColumnName { get; }
  
  short Index { get; }
}
