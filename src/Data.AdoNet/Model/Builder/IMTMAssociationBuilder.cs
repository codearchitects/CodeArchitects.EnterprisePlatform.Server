using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

/// <summary>
/// A builder that can be used to configure many-to-many relationships.
/// </summary>
/// <typeparam name="TFrom">The type of the "from" entity of the relationship.</typeparam>
/// <typeparam name="TTo">The type of the "to" entity of the relationship.</typeparam>
public interface IMTMAssociationBuilder<TFrom, TTo>
{
  /// <summary>
  /// Specifies the navigation property of the relationship.
  /// </summary>
  /// <param name="expression">An expression that represents the path to the navigation property.</param>
  /// <returns>The same <see cref="IOTMAssociationBuilder{TFrom, TTo}"/> that can be used to configure the many-to-many relationship.</returns>
  IMTMAssociationBuilder<TFrom, TTo> Navigation(Expression<Func<TFrom, IEnumerable<TTo>?>> expression);

  /// <summary>
  /// Specifies the navigation property of the relationship.
  /// </summary>
  /// <param name="navigationName">The name of the navigation property.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the many-to-many relationship.</returns>
  IMTMAssociationBuilder<TFrom, TTo> Navigation(string navigationName);

  /// <summary>
  /// Specifies the inverse navigation property of the relationship.
  /// </summary>
  /// <param name="expression">An expression that represents the path to the inverse navigation property.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the many-to-many relationship.</returns>
  IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(Expression<Func<TTo, IEnumerable<TFrom>?>> expression);

  /// <summary>
  /// Specifies the inverse navigation property of the relationship.
  /// </summary>
  /// <param name="navigationName">The name of the inverse navigation property.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the many-to-many relationship.</returns>
  IMTMAssociationBuilder<TFrom, TTo> InverseNavigation(string navigationName);

  /// <summary>
  /// Specifies the name of the junction table used to support the many-to-many relationship.
  /// </summary>
  /// <param name="tableName">The name of the junction table.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the many-to-many relationship.</returns>
  IMTMAssociationBuilder<TFrom, TTo> JunctionTable(string tableName);

  /// <summary>
  /// Specifies the names of the columns of the junction table used to support the many-to-many relationship.
  /// </summary>
  /// <param name="keyNames">The names of the columns.</param>
  /// <returns>The same <see cref="IOTOAssociationBuilder{TFrom, TTo}"/> that can be used to configure the many-to-many relationship.</returns>
  IMTMAssociationBuilder<TFrom, TTo> JunctionColumnNames(params string[] keyNames);
}
