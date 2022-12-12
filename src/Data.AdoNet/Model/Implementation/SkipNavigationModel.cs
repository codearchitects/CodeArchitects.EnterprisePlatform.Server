namespace CodeArchitects.Platform.Data.AdoNet.Model.Implementation;

internal abstract class SkipNavigationModel : NavigationModel, ISkipNavigationModel
{
  private SkipNavigationModel? _inverse;
  private readonly List<IKeyPair> _fromKeys;
  private readonly List<IKeyPair> _toKeys;

  protected SkipNavigationModel(int id, IEntityModel from, IEntityModel to, AssociationKind associationKind, CollectionKind collectionKind, bool isOnDependent, IEntityModel joinEntity)
    : base(id, from, to, associationKind, collectionKind, isOnDependent)
  {
    JoinEntity = joinEntity;
    _fromKeys = new();
    _toKeys = new();
  }

  protected override NavigationModel InverseCore => Inverse;

  public new SkipNavigationModel Inverse
  {
    get => _inverse ?? throw new InvalidOperationException("Inverse navigation was not set.");
    set => _inverse = value;
  }

  public IEntityModel JoinEntity { get; }

  public IReadOnlyList<IKeyPair> FromKeys => _fromKeys;

  public IReadOnlyList<IKeyPair> ToKeys => _toKeys;

  ISkipNavigationModel ISkipNavigationModel.Inverse => Inverse;

  public object CreateJoin(object from, object to)
  {
    Dictionary<string, object?> join = new(FromKeys.Count + ToKeys.Count);
    
    foreach (IKeyPair keyPair in FromKeys)
    {
      join.Add(keyPair.ForeignKeyColumn.Name, keyPair.PrimaryKeyColumn.GetValue(from));
    }
    foreach (IKeyPair keyPair in ToKeys)
    {
      join.Add(keyPair.ForeignKeyColumn.Name, keyPair.PrimaryKeyColumn.GetValue(to));
    }

    return join;
  }
}
