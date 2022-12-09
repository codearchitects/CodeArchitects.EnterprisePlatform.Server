namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class AssociationBuilder<TFrom, TTo> : IAggregationBuilder<TFrom, TTo>, ICompositionBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private readonly INavigationIdGenerator _idGenerator;
  private readonly AssociationKind _kind;
  private NavigationModelBuilder<TFrom, TTo>? _navigationBuilder;

  public AssociationBuilder(INavigationIdGenerator idGenerator, AssociationKind kind)
  {
    _idGenerator = idGenerator;
    _kind = kind;
  }

  public NavigationModelBuilder<TFrom, TTo> NavigationBuilder => _navigationBuilder ?? throw new ModelConfigurationException($"The multiplicity of the relationship '{typeof(TFrom).Name}' -> '{typeof(TTo).Name}' was not specified.");

  public IOTOAssociationBuilder<TFrom, TTo> OneToOne()
  {
    OTONavigationBuilder<TFrom, TTo> builder = new(_idGenerator, _kind);
    _navigationBuilder = builder;
    return builder;
  }

  public IOTMAssociationBuilder<TFrom, TTo> OneToMany()
  {
    OTMNavigationBuilder<TFrom, TTo> builder = new(_idGenerator, _kind);
    _navigationBuilder = builder;
    return builder;
  }

  public IMTMAssociationBuilder<TFrom, TTo> ManyToMany()
  {
    MTMNavigationBuilder<TFrom, TTo> builder = new(_idGenerator, _kind);
    _navigationBuilder = builder;
    return builder;
  }
}
