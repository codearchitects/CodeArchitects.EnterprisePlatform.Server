namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class AssociationBuilder<TFrom, TTo> : AssociationBuilder, IAggregationBuilder<TFrom, TTo>, ICompositionBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private readonly string _fromName;
  private readonly string _toName;
  private AssociationBuilder? _builder;

  public AssociationBuilder(string fromName, string toName)
  {
    _fromName = fromName;
    _toName = toName;
  }

  public override Association Build(AssociationKind kind)
  {
    return _builder is not null
      ? _builder.Build(kind)
      : throw new ModelConfigurationException($"The multiplicity of the relationship '{typeof(TFrom).Name}' -> '{typeof(TTo).Name}' was not specified.");
  }

  public IOTOAssociationBuilder<TFrom, TTo> OneToOne()
  {
    OTOAssociationBuilder<TFrom, TTo> builder = new();
    _builder = builder;
    return builder;
  }

  public IOTMAssociationBuilder<TFrom, TTo> OneToMany()
  {
    OTMAssociationBuilder<TFrom, TTo> builder = new();
    _builder = builder;
    return builder;
  }

  public IMTMAssociationBuilder<TFrom, TTo> ManyToMany()
  {
    MTMAssociationBuilder<TFrom, TTo> builder = new();
    _builder = builder;
    return builder;
  }
}
