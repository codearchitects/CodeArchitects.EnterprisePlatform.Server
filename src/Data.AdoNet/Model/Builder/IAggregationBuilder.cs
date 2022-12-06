namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

public interface IAggregationBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  IOTOAssociationBuilder<TFrom, TTo> OneToOne();
  IOTMAssociationBuilder<TFrom, TTo> OneToMany();
}
