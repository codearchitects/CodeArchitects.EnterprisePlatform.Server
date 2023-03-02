using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using CodeArchitects.Platform.Data.Features.Associations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Associations;

using TNavigation =
#if NET6_0_OR_GREATER
  IReadOnlyNavigation
#else
  INavigation
#endif
  ;

/// <summary>
/// Extension methods for configuring associations.
/// </summary>
public static class AssociationsAnnotationExtensions
{
  /// <summary>
  /// Specifies that an association is between two entities belonging to the same aggregates.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceCollectionBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> Aggregate<TPrincipalEntity, TDependentEntity>(this ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    if (EFCoreEnvironment.IsMigration)
      return builder;

    builder.Metadata.DependentToPrincipal?.IsIntraAggregate();
    builder.Metadata.PrincipalToDependent?.IsIntraAggregate();

    return builder;
  }

  /// <summary>
  /// Specifies that an association is between two entities belonging to different aggregates.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceCollectionBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> Associate<TPrincipalEntity, TDependentEntity>(this ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    if (EFCoreEnvironment.IsMigration)
      return builder;

    builder.Metadata.DependentToPrincipal?.IsInterAggregate();
    builder.Metadata.PrincipalToDependent?.IsInterAggregate();

    return builder;
  }

  /// <summary>
  /// Specifies that an association is between two entities belonging to the same aggregates.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceReferenceBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> Aggregate<TPrincipalEntity, TDependentEntity>(this ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    if (EFCoreEnvironment.IsMigration)
      return builder;

    builder.Metadata.DependentToPrincipal?.IsIntraAggregate();
    builder.Metadata.PrincipalToDependent?.IsIntraAggregate();

    return builder;
  }

  /// <summary>
  /// Specifies that an association is between two entities belonging to different aggregates.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceReferenceBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> Associate<TPrincipalEntity, TDependentEntity>(this ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    if (EFCoreEnvironment.IsMigration)
      return builder;

    builder.Metadata.DependentToPrincipal?.IsInterAggregate();
    builder.Metadata.PrincipalToDependent?.IsInterAggregate();

    return builder;
  }

  private static IAnnotation IsIntraAggregate(this IMutableNavigation navigation)
  {
    return navigation.AddAnnotation(AssociationsAnnotationNames.AssociationKind, AssociationKind.IntraAggregate);
  }

  private static IAnnotation IsInterAggregate(this IMutableNavigation navigation)
  {
    return navigation.AddAnnotation(AssociationsAnnotationNames.AssociationKind, AssociationKind.InterAggregate);
  }


  /// <summary>
  /// Indicates whether a navigation is between two entities belonging to the same aggregates.
  /// </summary>
  /// <param name="navigation">The navigation.</param>
  /// <returns><see langword="true"/> if the navigation represents an intra-aggregate association, <see langword="false"/> otherwise.</returns>
  public static bool IsIntraAggregate(this TNavigation navigation)
  {
    if (navigation is null)
      throw new ArgumentException(nameof(navigation));

    return navigation.Is(AssociationKind.IntraAggregate);
  }

  /// <summary>
  /// Indicates whether a navigation is between two entities belonging to different aggregates.
  /// </summary>
  /// <param name="navigation">The navigation.</param>
  /// <returns><see langword="true"/> if the navigation represents an inter-aggregate association, <see langword="false"/> otherwise.</returns>
  public static bool IsInterAggregate(this TNavigation navigation)
  {
    if (navigation is null)
      throw new ArgumentException(nameof(navigation));

    return navigation.Is(AssociationKind.InterAggregate);
  }

  private static bool Is(this TNavigation navigation, AssociationKind kind)
  {
    return
      navigation.TryGetAnnotationValue(AssociationsAnnotationNames.AssociationKind, out AssociationKind value) &&
      value == kind;
  }
}
