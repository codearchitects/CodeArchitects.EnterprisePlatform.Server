using CodeArchitects.Platform.Data.Features.Associations;
using System.Diagnostics;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class SimpleNavigationModel : NavigationModel, ISimpleNavigationModel
{
  private SimpleNavigationModel? _inverse;
  private readonly List<IKeyPair> _keyPairs;
  private NavigationEntityModel? _navigationEntity;

  protected SimpleNavigationModel(int id, EntityModel from, EntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
    _keyPairs = new();
  }

  public PrimaryKeyModel PrimaryKey => IsOnDependent ? To.PrimaryKey : From.PrimaryKey;

  public IReadOnlyList<IKeyPair> KeyPairs => _keyPairs;

  public new SimpleNavigationModel Inverse
  {
    get => _inverse ?? throw new InvalidOperationException("Inverse navigation was not set.");
    set => _inverse = value;
  }

  protected override NavigationModel InverseCore => Inverse;

  ISimpleNavigationModel ISimpleNavigationModel.Inverse => Inverse;

  IPrimaryKeyModel ISimpleNavigationModel.PrimaryKey => PrimaryKey;

  public NavigationEntityModel? NavigationEntity
  {
    get
    {
      if (IsOnDependent)
        return null;

      return _navigationEntity ?? throw new InvalidOperationException("Navigation entity was not set.");
    }
    set
    {
      Debug.Assert(_navigationEntity is null, "Navigation entity was set multiple times.");
      Debug.Assert(!IsOnDependent, "Attempted to set the navigation entity on a navigation on a dependent entity.");

      _navigationEntity = value;
    }
  }

  IEntityModel? ISimpleNavigationModel.NavigationEntity => NavigationEntity;

  public void AddForeignKey(IForeignKeyColumnModel foreignKeyColumn)
  {
    AddForeignKey(foreignKeyColumn, true);
  }

  private void AddForeignKey(IForeignKeyColumnModel foreignKeyColumn, bool addOnInverse)
  {
    _keyPairs.Add(new KeyPair(foreignKeyColumn, IsOnDependent));

    if (!addOnInverse)
      return;

    Inverse.AddForeignKey(foreignKeyColumn, false);
  }
}
