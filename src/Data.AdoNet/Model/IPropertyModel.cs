namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IPropertyModel : IPropertyBase
{
  string ColumnName { get; }
  short Index { get; }
}
