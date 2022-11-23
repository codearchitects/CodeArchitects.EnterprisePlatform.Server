namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IPropertyModel
{
  string Name { get; }
  string ColumnName { get; }
  short Index { get; }
}
