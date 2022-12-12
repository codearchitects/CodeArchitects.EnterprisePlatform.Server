using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class NavigationModelBuilder<TFrom, TTo> : NavigationModelBuilder
  where TFrom : class
  where TTo : class
{
  protected readonly INavigationIdGenerator _idGenerator;
  protected readonly AssociationKind _kind;
  protected MemberInfo? _directNavigationMember;
  protected MemberInfo? _inverseNavigationMember;

  public NavigationModelBuilder(INavigationIdGenerator idGenerator, AssociationKind kind)
  {
    _idGenerator = idGenerator;
    _kind = kind;
  }

  protected sealed override MemberInfo? DirectNavigationMember => _directNavigationMember;

  protected sealed override MemberInfo? InverseNavigationMember => _inverseNavigationMember;

  protected sealed override Type FromType => typeof(TFrom);

  protected sealed override Type ToType => typeof(TTo);

  public sealed override NavigationModel Build(DataModel dataModel)
  {
    if (!dataModel.TryGetEntity(FromType, out IEntityModel? fromEntity))
      throw new ModelConfigurationException($"An association '{FromType.Name}' -> '{ToType.Name}' was defined, but '{FromType.Name}' is not an entity.");

    if (!dataModel.TryGetEntity(ToType, out IEntityModel? toEntity))
      throw new ModelConfigurationException($"An association '{ToType.Name}' -> '{ToType.Name}' was defined, but '{ToType.Name}' is not an entity.");

    return Build(fromEntity, toEntity);
  }

  protected abstract NavigationModel Build(IEntityModel fromEntity, IEntityModel toEntity);

  protected static CollectionKind GetCollectionKind(Type memberType, Type entityType)
  {
    if (memberType.IsArray)
      throw new ModelConfigurationException("Plain arrays are not supported. Use other kind of collections instead (e.g., ICollection<T>, IList<T>, etc.).");

    return
      memberType.IsAssignableFrom(typeof(HashSet<>).MakeGenericType(entityType)) ? CollectionKind.HashSet :
      memberType.IsAssignableFrom(typeof(List<>).MakeGenericType(entityType)) ? CollectionKind.List :
      throw new ModelConfigurationException($"Collection type '{memberType}' is not supported. Use other kind of collections instead (e.g., ICollection<T>, IList<T>, etc.).");
  }
}
