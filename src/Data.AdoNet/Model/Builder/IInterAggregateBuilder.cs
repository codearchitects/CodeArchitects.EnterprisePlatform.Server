namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

/// <summary>
/// A builder that can be used to configure inter-aggregate relationships.
/// </summary>
/// <typeparam name="TFrom">The type of the "from" entity of the relationship.</typeparam>
/// <typeparam name="TTo">The type of the "to" entity of the relationship.</typeparam>
public interface IInterAggregateBuilder<TFrom, TTo>
  where TFrom : class
  where TTo : class
{
  /// <summary>
  /// Specifies that the relationship is one-to-one.
  /// </summary>
  /// <returns>An <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-one relationship.</returns>
  IOTOAssociationBuilder<TFrom, TTo> OneToOne();

  /// <summary>
  /// Specifies that the relationship is one-to-many.
  /// </summary>
  /// <returns>An <see cref="IOTMAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> OneToMany();

  /// <summary>
  /// Specifies that the relationship is many-to-many.
  /// </summary>
  /// <returns>An <see cref="IMTMAssociationBuilder{TFrom, TTo}"/> that can be used to configure the many-to-many relationship.</returns>
  IMTMAssociationBuilder<TFrom, TTo> ManyToMany();
}
