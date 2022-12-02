using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IForeignKeyPropertyModel : IPropertyModel
{
  short KeyIndex { get; }

  IPrimaryKeyPropertyModel PrimaryKeyProperty { get; }

  INavigationModel Navigation { get; }
}
