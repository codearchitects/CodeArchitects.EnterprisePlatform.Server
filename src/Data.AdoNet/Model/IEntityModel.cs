using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IEntityModel
{
  string Name { get; }
  string TableName { get; }
  Type Type { get; }

  IPrimaryKeyModel PrimaryKey { get; }

  IReadOnlyList<IPropertyModel> Properties { get; }

  bool TryGetNavigation(string navigation, [NotNullWhen(true)] out INavigationModel navigationModel);
}
