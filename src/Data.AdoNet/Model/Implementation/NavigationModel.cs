namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class NavigationModel : MemberModel, INavigationModel
{
  protected NavigationModel(int id, EntityModel from, EntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent)
  {
    Id = id;
    From = from;
    To = to;
    AssociationKind = associationKind;
    CollectionKind = collectionKind;
    IsOnDependent = isOnDependent;
  }

  protected abstract NavigationModel InverseCore { get; }

  public CollectionKind CollectionKind { get; }

  public int Id { get; }

  public AssociationKind AssociationKind { get; }

  public bool IsOnDependent { get; }

  public bool IsCollection => CollectionKind is not CollectionKind.None;

  public NavigationModel Inverse => InverseCore;

  public EntityModel From { get; }

  public EntityModel To { get; }

  INavigationModel INavigationModel.Inverse => Inverse;

  IEntityModel INavigationModel.From => From;

  IEntityModel INavigationModel.To => To;
}
