namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IPropertyModel : IPropertyModelBase
{
  string ColumnName { get; }
  
  short Index { get; }
}
