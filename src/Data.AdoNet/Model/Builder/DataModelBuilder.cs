using CodeArchitects.Platform.Data.AdoNet.Model.Implementation;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model.Builder;

internal class DataModelBuilder
{
  private readonly Dictionary<string, EntityModelBuilder> _entityBuilders;
	private readonly List<AssociationBuilder> _associationBuilders;

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

  public EntityModelBuilder<TEntity> GetEntityModelBuilder<TEntity>(string entityName)
		where TEntity : class
	{
		if (_entityBuilders.TryGetValue(entityName, out EntityModelBuilder? builder))
      return builder as EntityModelBuilder<TEntity>
				?? throw new ModelConstructionException($"Duplicate entity name '{entityName}' for entities of type '{typeof(TEntity).Name}' and '{builder.EntityType.Name}'.");

    EntityModelBuilder<TEntity> typedBuilder = new(entityName);
		_entityBuilders.Add(entityName, typedBuilder);

		return typedBuilder;
  }

	public AssociationBuilder<TFrom, TTo> GetAssociationBuilder<TFrom, TTo>()
		where TFrom : class
		where TTo : class
	{
		AssociationBuilder<TFrom, TTo> builder = new();
		_associationBuilders.Add(builder);

		return builder;
	}

	private IEntityModel BuildEntity(EntityModelBuilder entityBuilder)
	{
		List<AccessibleMemberModelComponent> memberComponents = entityBuilder.EntityType
			.GetMembers(BindingFlags.Instance | BindingFlags.Public)
			.Where(member => !entityBuilder.IgnoredMembers.Contains(member))
			.Select(AccessibleMemberModelComponent.Create)
			.ToList();

		HashSet<ColumnModel> columns = new();
		foreach (AccessibleMemberModelComponent memberComponent in memberComponents)
		{
			if (Configuration.IsSupportedColumnType(memberComponent.Type))
			{
			}
		}

		throw null!;
	}
}
