namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IForeignKeyModel
{
  bool IsComposite { get; }

  Type Type { get; }
  
  IReadOnlyList<IForeignKeyPropertyModel> Properties { get; }
}
