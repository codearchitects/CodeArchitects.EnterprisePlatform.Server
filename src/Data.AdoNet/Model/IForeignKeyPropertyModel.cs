namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IForeignKeyPropertyModel : IPropertyModel
{
  short KeyIndex { get; }
  INavigationModel Navigation { get; }
}
