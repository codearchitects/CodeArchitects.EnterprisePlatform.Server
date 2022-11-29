namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IPropertyModel : IPropertyModelBase
{
  string ColumnName { get; }
  short Index { get; }
}
