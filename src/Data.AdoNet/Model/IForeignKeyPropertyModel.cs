namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IForeignKeyPropertyModel : IPropertyModel
{
  short KeyIndex { get; }

  INavigationModel Navigation { get; }
}
