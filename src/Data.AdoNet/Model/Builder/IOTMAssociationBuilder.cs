using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

/// <summary>
/// A builder that can be used to configure one-to-many relationships.
/// </summary>
/// <typeparam name="TFrom">The type of the "from" entity of the relationship.</typeparam>
/// <typeparam name="TTo">The type of the "to" entity of the relationship.</typeparam>
public interface IOTMAssociationBuilder<TFrom, TTo>
{
  /// <summary>
  /// Specifies the navigation property of the relationship.
  /// </summary>
  /// <param name="expression">An expression that represents the path to the navigation property.</param>
  /// <returns>The same <see cref="IOTMAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression);

  /// <summary>
  /// Specifies the navigation property of the relationship.
  /// </summary>
  /// <param name="navigationName">The name of the navigation property.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> Navigation(string navigationName);

  /// <summary>
  /// Specifies the inverse navigation property of the relationship.
  /// </summary>
  /// <param name="expression">An expression that represents the path to the inverse navigation property.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, TFrom?>> expression);

  /// <summary>
  /// Specifies the inverse navigation property of the relationship.
  /// </summary>
  /// <param name="navigationName">The name of the inverse navigation property.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName);

  /// <summary>
  /// Specifies the foreign key property or properties of the navigation.
  /// </summary>
  /// <remarks>
  /// If the foreign key is a simple key, then a member expression can be used: <c>entity => entity.ForeignKey</c>.
  /// A composite foreign key can be specified by using an anonymous object creation: <c>entity => new { entity.ForeignKey1, entity.ForeignKey2 }</c>.
  /// </remarks>
  /// <typeparam name="TForeignKey">The foreign key type.</typeparam>
  /// <param name="expression">An expression that represents the path to the foreign key.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> ForeignKey<TForeignKey>(Expression<Func<TTo, TForeignKey>> expression);

  /// <summary>
  /// Specifies the foreign key property or properties of the navigation.
  /// </summary>
  /// <param name="keyNames">The names of the foreign keys.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> ForeignKey(params Name[] keyNames);

  /// <summary>
  /// Specifies the foreign key property or properties of the navigation.
  /// </summary>
  /// <param name="keyNames">The names of the foreign key properties.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the one-to-many relationship.</returns>
  IOTMAssociationBuilder<TFrom, TTo> ForeignKey(params string[] keyNames);
}
