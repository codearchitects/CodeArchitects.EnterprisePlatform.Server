namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class AssociationBuilder<TFrom, TTo> : AssociationBuilder, IAssociationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  private AssociationBuilder? _builder;

  public override Association Build()
  {
    if (_builder is null)
      throw new InvalidOperationException("");

    return _builder.Build();
  }

  public IAssociationBuilderOTO<TFrom, TTo> OneToOne()
  {
    throw new NotImplementedException();
  }

  public IAssociationBuilderOTM<TFrom, TTo> OneToMany()
  {
    throw new NotImplementedException();
  }

  public IAssociationBuilderMTM<TFrom, TTo> ManyToMany()
  {
    throw new NotImplementedException();
  }
}
