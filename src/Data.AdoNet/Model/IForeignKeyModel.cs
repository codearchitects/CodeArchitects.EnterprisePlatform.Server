namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IForeignKeyModel
{
  bool IsComposite { get; }
  Type Type { get; }
  IReadOnlyList<IForeignKeyPropertyModel> Properties { get; }
}
