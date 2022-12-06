using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class DataModelBuilder
{
  private readonly Dictionary<Type, EntityModelBuilder> _entityBuilders;
	private readonly Dictionary<(Type, Type), AssociationBuilder> _associationBuilders;

  public DataModelBuilder()
	{
		_entityBuilders = new();
		_associationBuilders = new();
  }

  public DataModel Build()
  {
		DataModel model = new();

		foreach (EntityModelBuilder entityBuilder in _entityBuilders.Values)
		{
			model.AddEntity(BuildEntity(entityBuilder));
		}

		return model;
  }

  public EntityModelBuilder<TEntity> GetEntityModelBuilder<TEntity>()
		where TEntity : class
	{
		if (_entityBuilders.TryGetValue(typeof(TEntity), out EntityModelBuilder? builder))
			return (EntityModelBuilder<TEntity>)builder;

		EntityModelBuilder<TEntity> typedBuilder = new();
		_entityBuilders.Add(typeof(TEntity), typedBuilder);

		return typedBuilder;
  }

	public AssociationBuilder<TFrom, TTo> Get

	private IEntityModel BuildEntity(EntityModelBuilder entityBuilder)
	{
		List<AccessibleMemberModelComponent> memberComponents = entityBuilder.EntityType
			.GetMembers(BindingFlags.Instance | BindingFlags.Public)
			.Where(member => !entityBuilder.IgnoredMembers.Contains(member))
			.Select(AccessibleMemberModelComponent.Create)
			.ToList();

		entityBuilder.ke

		HashSet<ColumnModel> columns = new();
		foreach (AccessibleMemberModelComponent memberComponent in memberComponents)
		{
			if (Configuration.IsSupportedColumnType(memberComponent.Type))
			{
			}
			else if (_entityBuilders.ContainsKey(memberComponent.Type))
			{

			}
		}
	}
}
