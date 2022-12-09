namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class AssociationBuilder<TFrom, TTo> : IAggregationBuilder<TFrom, TTo>, ICompositionBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private readonly INavigationIdGenerator _idGenerator;
  private readonly AssociationKind _kind;
  private readonly string _fromEntityName;
  private readonly string _toEntityName;
  private NavigationModelBuilder? _navigationBuilder;

  public AssociationBuilder(INavigationIdGenerator idGenerator, AssociationKind kind, string fromEntityName, string toEntityName)
  {
    _idGenerator = idGenerator;
    _kind = kind;
    _fromEntityName = fromEntityName;
    _toEntityName = toEntityName;
  }

  public NavigationModelBuilder NavigationBuilder => _navigationBuilder ?? throw new ModelConfigurationException($"The multiplicity of the relationship '{_fromEntityName}' -> '{_toEntityName}' was not specified.");

  public IOTOAssociationBuilder<TFrom, TTo> OneToOne()
  {
    OTONavigationBuilder<TFrom, TTo> builder = new(_idGenerator, _kind, _fromEntityName, _toEntityName);
    _navigationBuilder = builder;
    return builder;
  }

  public IOTMAssociationBuilder<TFrom, TTo> OneToMany()
  {
    OTMNavigationBuilder<TFrom, TTo> builder = new(_idGenerator, _kind, _fromEntityName, _toEntityName);
    _navigationBuilder = builder;
    return builder;
  }

  public IMTMAssociationBuilder<TFrom, TTo> ManyToMany()
  {
    MTMNavigationBuilder<TFrom, TTo> builder = new(_idGenerator, _kind, _fromEntityName, _toEntityName);
    _navigationBuilder = builder;
    return builder;
  }
}
