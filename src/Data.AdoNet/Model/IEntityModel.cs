namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IEntityModel
{
  string Name { get; }
  Type Type { get; }

  IPrimaryKeyModel PrimaryKey { get; }

  IReadOnlyList<IPropertyModel> Properties { get; }
}
