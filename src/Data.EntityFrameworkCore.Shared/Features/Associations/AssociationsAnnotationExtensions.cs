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
  /// Specifies that an association is a composition.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceCollectionBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> AsComposition<TPrincipalEntity, TDependentEntity>(this ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    builder.Metadata.DependentToPrincipal?.IsComposition();
    builder.Metadata.PrincipalToDependent?.IsComposition();

    return builder;
  }

  /// <summary>
  /// Specifies that an association is an aggregation.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceCollectionBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> AsAggregation<TPrincipalEntity, TDependentEntity>(this ReferenceCollectionBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    builder.Metadata.DependentToPrincipal?.IsAggregation();
    builder.Metadata.PrincipalToDependent?.IsAggregation();

    return builder;
  }

  /// <summary>
  /// Specifies that an association is a composition.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceReferenceBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> AsComposition<TPrincipalEntity, TDependentEntity>(this ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    builder.Metadata.DependentToPrincipal?.IsComposition();
    builder.Metadata.PrincipalToDependent?.IsComposition();

    return builder;
  }

  /// <summary>
  /// Specifies that an association is an aggregation.
  /// </summary>
  /// <typeparam name="TPrincipalEntity">The principal entity type in this relationship.</typeparam>
  /// <typeparam name="TDependentEntity">The dependent entity type in this relationship.</typeparam>
  /// <param name="builder">The relationship builder.</param>
  /// <returns>The same <see cref="ReferenceReferenceBuilder{TPrincipalEntity, TDependentEntity}"/> for further configuration.</returns>
  public static ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> AsAggregation<TPrincipalEntity, TDependentEntity>(this ReferenceReferenceBuilder<TPrincipalEntity, TDependentEntity> builder)
    where TPrincipalEntity : class
    where TDependentEntity : class
  {
    if (builder is null)
      throw new ArgumentException(nameof(builder));

    builder.Metadata.DependentToPrincipal?.IsAggregation();
    builder.Metadata.PrincipalToDependent?.IsAggregation();

    return builder;
  }

  private static IAnnotation IsComposition(this IMutableNavigation navigation)
  {
    return navigation.AddAnnotation(AssociationsAnnotationNames.AssociationKind, RuntimeAnnotationWrapper.Create(AssociationKind.Composition));
  }

  private static IAnnotation IsAggregation(this IMutableNavigation navigation)
  {
    return navigation.AddAnnotation(AssociationsAnnotationNames.AssociationKind, RuntimeAnnotationWrapper.Create(AssociationKind.Aggregation));
  }


  /// <summary>
  /// Indicates whether a navigation is a composition.
  /// </summary>
  /// <param name="navigation">The navigation.</param>
  /// <returns><c>true</c> if the navigation represents a composition, <c>false</c> otherwise.</returns>
  public static bool IsComposition(this TNavigation navigation)
  {
    if (navigation is null)
      throw new ArgumentException(nameof(navigation));

    return navigation.Is(AssociationKind.Composition);
  }

  /// <summary>
  /// Indicates whether a navigation is an aggregation.
  /// </summary>
  /// <param name="navigation">The navigation.</param>
  /// <returns><c>true</c> if the navigation represents an aggregation, <c>false</c> otherwise.</returns>
  public static bool IsAggregation(this TNavigation navigation)
  {
    if (navigation is null)
      throw new ArgumentException(nameof(navigation));

    return navigation.Is(AssociationKind.Aggregation);
  }

  private static bool Is(this TNavigation navigation, AssociationKind kind)
  {
    return
      navigation.TryGetRuntimeAnnotationValue(AssociationsAnnotationNames.AssociationKind, out AssociationKind value) &&
      value == kind;
  }
}
