using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class OTONavigationBuilder<TFrom, TTo> : NavigationModelBuilder<TFrom, TTo>, IOTOAssociationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private readonly List<Name> _foreignKeyNames;

  public OTONavigationBuilder(INavigationIdGenerator idGenerator, AssociationKind kind)
    : base(idGenerator, kind)
  {
    _foreignKeyNames = new();
  }

  public override IReadOnlyCollection<Name> ForeignKeyNames => _foreignKeyNames;

  protected override (NavigationModel Direct, NavigationModel Inverse) Build(IEntityModel fromEntity, IEntityModel toEntity)
  {
    SimpleNavigationModel direct;
    SimpleNavigationModel inverse;

    if (_directNavigationMember is not null)
    {
      AccessibleMemberComponent<object?> memberComponent = AccessibleMemberComponent<object?>.Create(_directNavigationMember);
      direct = new AccessibleSimpleNavigationModel(memberComponent, _idGenerator.GetNextId(), fromEntity, toEntity, _kind, CollectionKind.None, false);
    }
    else
    {
      direct = new HiddenSimpleNavigationModel(new HiddenMemberComponent<object?>(ToType), _idGenerator.GetNextId(), fromEntity, toEntity, _kind, CollectionKind.None, false);
    }

    if (_inverseNavigationMember is not null)
    {
      AccessibleMemberComponent<object?> memberComponent = AccessibleMemberComponent<object?>.Create(_inverseNavigationMember);
      inverse = new AccessibleSimpleNavigationModel(memberComponent, _idGenerator.GetNextId(), toEntity, fromEntity, _kind, CollectionKind.None, true);
    }
    else
    {
      inverse = new HiddenSimpleNavigationModel(new HiddenMemberComponent<object?>(FromType), _idGenerator.GetNextId(), toEntity, fromEntity, _kind, CollectionKind.None, true);
    }

    direct.Inverse = inverse;
    inverse.Inverse = direct;

    return (direct, inverse);
  }

  public IOTOAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression)
  {
    if (!TryGetMemberAndTypeFromExpression(expression.Body, out _inverseNavigationMember, out _))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName)
  {
    _inverseNavigationMember = GetMember(typeof(TTo), navigationName);

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, TTo?>> expression)
  {
    if (!TryGetMemberAndTypeFromExpression(expression.Body, out _inverseNavigationMember, out _))
      throw new ModelConfigurationException("The expression must be 'x => x.Member'.");

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> Navigation(string navigationName)
  {
    _directNavigationMember = GetMember(typeof(TFrom), navigationName);

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression)
  {
    _foreignKeyNames.Clear();
    _foreignKeyNames.AddRange(GetKeyNames(expression.Body));

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey(params string[] keyNames)
  {
    CheckKeyArity(keyNames.Length);

    _foreignKeyNames.Clear();

    foreach (string keyName in keyNames)
    {
      _ = GetMemberAndType(typeof(TFrom), keyName);

      _foreignKeyNames.Add(keyName);
    }

    return this;
  }

  public IOTOAssociationBuilder<TFrom, TTo> UsingForeignKey(params Name[] keyNames)
  {
    CheckKeyArity(keyNames.Length);

    _foreignKeyNames.Clear();

    foreach (Name keyName in keyNames)
    {
      if (!keyName.IsColumnName)
      {
        _ = GetMemberAndType(typeof(TFrom), keyName.Value);
      }

      _foreignKeyNames.Add(keyName);
    }

    return this;
  }
}
