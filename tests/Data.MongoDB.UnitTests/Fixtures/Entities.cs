using CodeArchitects.Platform.Data.MongoDB.Model;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

class EntityWithIdProperty
{
  public Guid Id { get; set; }
}

[Table("Entities")]
class EntityWithIdPropertyAndTableAttribute
{
  public Guid Id { get; set; }
}

class EntityWithBsonIdAttribute
{
  [BsonId]
  public Guid Identifier { get; set; }
}

class MalformedEntity
{
  public int Number { get; set; }
}

internal static class Models
{
  public static readonly IEntityModel EntityWithIdProperty = CreateEntityWithIdProperty();
  public static readonly IEntityModel EntityWithIdPropertyAndTableAttribute = CreateEntityWithIdPropertyAndTableAttribute();
  public static readonly IEntityModel EntityWithBsonIdAttribute = CreateEntityWithBsonIdAttribute();

  private static IEntityModel CreateEntityWithIdProperty()
  {
    IKeyModel keyModel = Mock.Of<IKeyModel>(model =>
      model.Name == "Id" &&
      model.Type == typeof(Guid));
    Type type = typeof(EntityWithIdProperty);

    return Mock.Of<IEntityModel>(model =>
      model.CollectionName == type.Name &&
      model.Type == type &&
      model.Key == keyModel &&
      model.Key.Name == keyModel.Name);
  }

  private static IEntityModel CreateEntityWithIdPropertyAndTableAttribute()
  {
    IKeyModel keyModel = Mock.Of<IKeyModel>(model =>
      model.Name == "Id" &&
      model.Type == typeof(Guid));

    return Mock.Of<IEntityModel>(model =>
      model.CollectionName == "Entities" &&
      model.Type == typeof(EntityWithIdPropertyAndTableAttribute) &&
      model.Key == keyModel &&
      model.Key.Name == keyModel.Name);
  }

  private static IEntityModel CreateEntityWithBsonIdAttribute()
  {
    IKeyModel keyModel = Mock.Of<IKeyModel>(model =>
      model.Name == "Identifier" &&
      model.Type == typeof(Guid));
    Type type = typeof(EntityWithBsonIdAttribute);

    return Mock.Of<IEntityModel>(model =>
      model.CollectionName == type.Name &&
      model.Type == type &&
      model.Key == keyModel &&
      model.Key.Name == keyModel.Name);
  }
}

internal static class KeyModels
{
  public static readonly IKeyModel IdProperty = CreateIdProperty();

  private static IKeyModel CreateIdProperty()
  {
    return Mock.Of<IKeyModel>(model =>
      model.Name == "Id" &&
      model.Type == typeof(Guid));
  }
}

internal static class PredicateTemplates
{
  public static readonly Expression<Func<EntityWithIdProperty, bool>> EntityWithIdPropertyKeyTemplate = CreateEntityWithIdPropertyKeyTemplate();
  public static readonly Expression<Func<EntityWithIdProperty, bool>> EntityWithIdPropertyEntityTemplate = CreateEntityWithIdPropertyEntityTemplate();

  private static Expression<Func<EntityWithIdProperty, bool>> CreateEntityWithIdPropertyKeyTemplate()
  {
    string keyName = "Id";
    Type entityType = typeof(EntityWithIdProperty);
    Type keyType = typeof(Guid);
    ParameterExpression entityParam = Expression.Parameter(entityType, "entity");

    return Expression.Lambda<Func<EntityWithIdProperty, bool>>(
      body: Expression.Equal(
        left: Expression.Property(
          expression: entityParam,
          propertyName: keyName),
        right: Expression.Variable(keyType)),
      parameters: entityParam);
  }

  private static Expression<Func<EntityWithIdProperty, bool>> CreateEntityWithIdPropertyEntityTemplate()
  {
    string keyName = "Id";
    Type entityType = typeof(EntityWithIdProperty);
    ParameterExpression entityParameter = Expression.Parameter(entityType, "entity");

    return Expression.Lambda<Func<EntityWithIdProperty, bool>>(
      body: Expression.Equal(
        left: Expression.Property(
          expression: entityParameter,
          propertyName: keyName),
        right: Expression.Property(
          expression: Expression.Variable(entityType),
          propertyName: keyName)),
      parameters: entityParameter);
  }
}
