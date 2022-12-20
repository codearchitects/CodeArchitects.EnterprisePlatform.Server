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

  IEntityModel ISkipNavigationModel.JunctionEntity => JoinEntity;

  public void AddFromJoinColumn(IPrimaryKeyColumnModel primaryKeyColumn, string name)
  {
    AddFromJoinColumn(primaryKeyColumn, name, true);
  }

  private void AddFromJoinColumn(IPrimaryKeyColumnModel primaryKeyColumn, string name, bool addOnInverse)
  {
    JoinColumnModel joinColumn = JoinColumnModel.Create(name, primaryKeyColumn.Type, (short)JoinEntity.Columns.Count);
    JoinKeyPair keyPair = new(primaryKeyColumn, joinColumn, true);
    _fromKeyPairs.Add(keyPair);

    if (addOnInverse)
    {
      JoinEntity.AddColumn(joinColumn);
      Inverse.AddToJoinColumn(primaryKeyColumn, name, false);
    }
  }

  public void AddToJoinColumn(IPrimaryKeyColumnModel primaryKeyColumn, string name)
  {
    AddToJoinColumn(primaryKeyColumn, name, true);
  }

  private void AddToJoinColumn(IPrimaryKeyColumnModel primaryKeyColumn, string name, bool addOnInverse)
  {
    JoinColumnModel joinColumn = JoinColumnModel.Create(name, primaryKeyColumn.Type, (short)JoinEntity.Columns.Count);
    JoinKeyPair keyPair = new(primaryKeyColumn, joinColumn, false);
    _toKeyPairs.Add(keyPair);

    if (addOnInverse)
    {
      JoinEntity.AddColumn(joinColumn);
      Inverse.AddFromJoinColumn(primaryKeyColumn, name, false);
    }
  }

  public object CreateJunction(object from, object to)
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
