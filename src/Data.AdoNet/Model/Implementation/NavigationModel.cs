namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class NavigationModel : MemberModel, INavigationModel
{
  protected NavigationModel(int id, EntityModel from, EntityModel to, AssociationKind associationKind)
  {
    Id = id;
    From = from;
    To = to;
    AssociationKind = associationKind;
  }

  public abstract NavigationModel Inverse { get; }

  public int Id { get; }

  public AssociationKind AssociationKind { get; }

  public bool IsOnDependent => throw new NotImplementedException();

  public bool IsCollection => throw new NotImplementedException();

  public EntityModel From { get; }

  public EntityModel To { get; }

  public IPrimaryKeyModel PrimaryKey => throw new NotImplementedException();

  public IForeignKeyModel ForeignKey => throw new NotImplementedException();

  public CollectionKind CollectionKind => throw new NotImplementedException();

  #region Explicit implementation

  IEntityModel INavigationModel.From => throw new NotImplementedException();

  IEntityModel INavigationModel.To => throw new NotImplementedException();

  INavigationModel INavigationModel.Inverse => throw new NotImplementedException();

  #endregion
}
