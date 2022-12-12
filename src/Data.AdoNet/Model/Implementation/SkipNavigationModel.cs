namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class SkipNavigationModel : NavigationModel, ISkipNavigationModel
{
  private SkipNavigationModel? _inverse;
  private readonly List<IKeyPair> _fromKeyPairs;
  private readonly List<IKeyPair> _toKeyPairs;

  protected SkipNavigationModel(int id, EntityModel from, EntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent, JoinEntityModel joinEntity)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
    JoinEntity = joinEntity;
    _fromKeyPairs = new();
    _toKeyPairs = new();
  }

  protected override NavigationModel InverseCore => Inverse;

  public new SkipNavigationModel Inverse
  {
    get => _inverse ?? throw new InvalidOperationException("Inverse navigation was not set.");
    set => _inverse = value;
  }

  public JoinEntityModel JoinEntity { get; }

  public IReadOnlyList<IKeyPair> FromKeyPairs => _fromKeyPairs;

  public IReadOnlyList<IKeyPair> ToKeyPairs => _toKeyPairs;

  ISkipNavigationModel ISkipNavigationModel.Inverse => Inverse;

  IEntityModel ISkipNavigationModel.JoinEntity => JoinEntity;

  public void AddFromForeignKey(IForeignKeyColumnModel foreignKeyColumn)
  {
    AddFromForeignKey(foreignKeyColumn, true);
  }

  public void AddFromForeignKey(IForeignKeyColumnModel foreignKeyColumn, bool addOnInverse)
  {
    _fromKeyPairs.Add(new KeyPair(foreignKeyColumn, IsOnDependent));

    if (!addOnInverse)
      return;

    Inverse.AddFromForeignKey(foreignKeyColumn, false);
  }

  public void AddToForeignKey(IForeignKeyColumnModel foreignKeyColumn)
  {
    AddToForeignKey(foreignKeyColumn, true);
  }

  public void AddToForeignKey(IForeignKeyColumnModel foreignKeyColumn, bool addOnInverse)
  {
    _toKeyPairs.Add(new KeyPair(foreignKeyColumn, IsOnDependent));

    if (!addOnInverse)
      return;

    Inverse.AddToForeignKey(foreignKeyColumn, false);
  }

  public object CreateJoin(object from, object to)
  {
    Dictionary<string, object?> join = new(FromKeyPairs.Count + ToKeyPairs.Count);
    
    foreach (IKeyPair keyPair in FromKeyPairs)
    {
      join.Add(keyPair.ForeignKeyColumn.Name, keyPair.PrimaryKeyColumn.GetValue(from));
    }
    foreach (IKeyPair keyPair in ToKeyPairs)
    {
      join.Add(keyPair.ForeignKeyColumn.Name, keyPair.PrimaryKeyColumn.GetValue(to));
    }

    return join;
  }
}
