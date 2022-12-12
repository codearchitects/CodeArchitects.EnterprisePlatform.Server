using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class MTMNavigationBuilder<TFrom, TTo> : NavigationModelBuilder<TFrom, TTo>, IMTMAssociationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private CollectionKind _directCollectionKind = CollectionKind.Unknown;
  private CollectionKind _inverseCollectionKind = CollectionKind.Unknown;
  private string? _tableName;
  private readonly List<Name> _columnNames;

  public MTMNavigationBuilder(INavigationIdGenerator idGenerator, AssociationKind kind)
    : base(idGenerator, kind)
  {
    _columnNames = new();
  }

  public override IReadOnlyCollection<Name> ForeignKeyNames => _columnNames;

  protected override NavigationModel Build(EntityModel fromEntity, EntityModel toEntity)
  {
    JoinEntityModel joinEntity = new(_tableName ?? $"{typeof(TFrom).Name}{typeof(TTo).Name}");
    SkipNavigationModel direct;
    SkipNavigationModel inverse;

    if (_directNavigationMember is not null)
    {
      AccessibleMemberComponent<object?> memberComponent = AccessibleMemberComponent<object?>.Create(_directNavigationMember);
      direct = new AccessibleSkipNavigationModel(memberComponent, _idGenerator.GetNextId(), fromEntity, toEntity, _kind, _directCollectionKind, false, joinEntity);
    }
    else
    {
      direct = new HiddenSkipNavigationModel(new HiddenMemberComponent<object?>(ToType), _idGenerator.GetNextId(), fromEntity, toEntity, _kind, _directCollectionKind, false, joinEntity);
    }

    if (_inverseNavigationMember is not null)
    {
      AccessibleMemberComponent<object?> memberComponent = AccessibleMemberComponent<object?>.Create(_inverseNavigationMember);
      inverse = new AccessibleSkipNavigationModel(memberComponent, _idGenerator.GetNextId(), toEntity, fromEntity, _kind, _inverseCollectionKind, false, joinEntity);
    }
    else
    {
      inverse = new HiddenSkipNavigationModel(new HiddenMemberComponent<object?>(FromType), _idGenerator.GetNextId(), toEntity, fromEntity, _kind, _inverseCollectionKind, false, joinEntity);
    }

    direct.Inverse = inverse;
    inverse.Inverse = direct;

    return direct;
  }

  public IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, IEnumerable<TFrom>?>> expression)
  {
    if (!TryGetMemberAndTypeFromExpression(expression.Body, out _inverseNavigationMember, out Type? memberType))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _inverseCollectionKind = GetCollectionKind(memberType, typeof(TFrom));

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName)
  {
    (_inverseNavigationMember, Type memberType) = GetMemberAndType(typeof(TTo), navigationName);
    _inverseCollectionKind = GetCollectionKind(memberType, typeof(TFrom));

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression)
  {
    if (!TryGetMemberAndTypeFromExpression(expression.Body, out _directNavigationMember, out Type? memberType))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    _directCollectionKind = GetCollectionKind(memberType, typeof(TTo));

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> Navigation(string navigationName)
  {
    (_directNavigationMember, Type memberType) = GetMemberAndType(typeof(TFrom), navigationName);
    _directCollectionKind = GetCollectionKind(memberType, typeof(TTo));

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> UsingJoinColumnNames(params string[] columnNames)
  {
    _columnNames.Clear();
    _columnNames.AddRange(columnNames.Select(name => new Name(new ColumnName(name))));

    return this;
  }

  public IMTMAssociationBuilder<TFrom, TTo> UsingJoinTable(string tableName)
  {
    _tableName = tableName;

    return this;
  }
}
