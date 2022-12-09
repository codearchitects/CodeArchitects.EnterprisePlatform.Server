using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal abstract class NavigationModelBuilder<TFrom, TTo> : NavigationModelBuilder
  where TFrom : class
  where TTo : class
{
  protected readonly INavigationIdGenerator _idGenerator;
  protected readonly AssociationKind _kind;
  protected readonly string _fromEntityName;
  protected readonly string _toEntityName;
  protected MemberInfo? _directNavigationMember;
  protected MemberInfo? _inverseNavigationMember;

  public NavigationModelBuilder(INavigationIdGenerator idGenerator, AssociationKind kind, string fromEntityName, string toEntityName)
  {
    _idGenerator = idGenerator;
    _kind = kind;
    _fromEntityName = fromEntityName;
    _toEntityName = toEntityName;
  }

  protected sealed override string FromEntityName => _fromEntityName;

  protected sealed override string ToEntityName => _toEntityName;

  protected sealed override MemberInfo? DirectNavigationMember => _directNavigationMember;

  protected sealed override MemberInfo? InverseNavigationMember => _inverseNavigationMember;

  protected Type FromType => typeof(TFrom);

  protected Type ToType => typeof(TTo);

  public sealed override (NavigationModel Direct, NavigationModel Inverse) Build(DataModel dataModel)
  {
    if (!dataModel.TryGetEntity(_fromEntityName, out IEntityModel? fromEntity) || fromEntity.Type != FromType)
      throw new ModelConfigurationException($"An association '{_fromEntityName}' -> '{_toEntityName}' was defined, but '{_fromEntityName}' is not an entity of type '{FromType}'.");

    if (!dataModel.TryGetEntity(_toEntityName, out IEntityModel? toEntity) || toEntity.Type != ToType)
      throw new ModelConfigurationException($"An association '{_toEntityName}' -> '{_toEntityName}' was defined, but '{_toEntityName}' is not an entity of type '{ToType}'.");

    return Build(fromEntity, toEntity);
  }

  protected abstract (NavigationModel Direct, NavigationModel Inverse) Build(IEntityModel fromEntity, IEntityModel toEntity);

  protected static CollectionKind GetCollectionKind(Type memberType)
  {
    if (memberType.IsArray)
      throw new ModelConfigurationException("Plain arrays are not supported. Use other kind of collections instead (e.g., ICollection<>, IList<>, etc.).");

    return
      memberType.IsAssignableFrom(typeof(HashSet<>).MakeGenericType(typeof(TTo))) ? CollectionKind.HashSet :
      memberType.IsAssignableFrom(typeof(List<>).MakeGenericType(typeof(TTo))) ? CollectionKind.List :
      throw new ModelConfigurationException($"Collection type '{memberType}' is not supported. Use other kind of collections instead (e.g., ICollection<>, IList<>, etc.).");
  }
}
