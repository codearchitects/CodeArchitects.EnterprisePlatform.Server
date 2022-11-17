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

public static class AnnotationExtensions
{
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


  public static bool IsComposition(this TNavigation navigation)
  {
    if (navigation is null)
      throw new ArgumentException(nameof(navigation));

    return navigation.Is(AssociationKind.Composition);
  }

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
