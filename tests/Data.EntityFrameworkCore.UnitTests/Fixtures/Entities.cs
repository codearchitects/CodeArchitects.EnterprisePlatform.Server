using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;

class EntityWithSimplePropertyKey
{
  public int Id { get; set; }
}

class EntityWithSimpleFieldKey
{
  public int Id;
}

class EntityWithSimpleShadowKey
{
}

class EntityWithCompositeKey
{
  public int Id1 { get; set; }
  public string? Id2 { get; set; }
}

internal static class FindPredicateTemplates
{
  public static readonly Expression<Func<EntityWithSimplePropertyKey, bool>> SimplePropertyKeyTemplate = CreateSimplePropertyKeyTemplate();
  public static readonly Expression<Func<EntityWithSimpleFieldKey, bool>> SimpleFieldKeyTemplate = CreateSimpleFieldKeyTemplate();
  public static readonly Expression<Func<EntityWithSimpleShadowKey, bool>> SimpleShadowKeyTemplate = CreateSimpleShadowKeyTemplate();
  public static readonly Expression<Func<EntityWithCompositeKey, bool>> CompositeKeyTemplate = CreateCompositeKeyTemplate();

  private static Expression<Func<EntityWithSimplePropertyKey, bool>> CreateSimplePropertyKeyTemplate()
  {
    ParameterExpression entity = Expression.Parameter(typeof(EntityWithSimplePropertyKey), nameof(entity));
    return Expression.Lambda<Func<EntityWithSimplePropertyKey, bool>>(
      body: Expression.Equal(
        left: Expression.Property(
          expression: entity,
          propertyName: nameof(EntityWithSimplePropertyKey.Id)),
        right: Expression.Variable(typeof(int))),
      entity);
  }

  private static Expression<Func<EntityWithSimpleFieldKey, bool>> CreateSimpleFieldKeyTemplate()
  {
    ParameterExpression entity = Expression.Parameter(typeof(EntityWithSimpleFieldKey), nameof(entity));
    return Expression.Lambda<Func<EntityWithSimpleFieldKey, bool>>(
      body: Expression.Equal(
        left: Expression.Field(
          expression: entity,
          fieldName: nameof(EntityWithSimpleFieldKey.Id)),
        right: Expression.Variable(typeof(int))),
      entity);
  }

  private static Expression<Func<EntityWithSimpleShadowKey, bool>> CreateSimpleShadowKeyTemplate()
  {
    ParameterExpression entity = Expression.Parameter(typeof(EntityWithSimpleShadowKey), nameof(entity));
    return Expression.Lambda<Func<EntityWithSimpleShadowKey, bool>>(
      body: Expression.Equal(
        left: ExpressionHelpers.MakeShadowPropertyAccess(
          entity: entity,
          propertyName: "Id",
          propertyType: typeof(int)),
        right: Expression.Variable(typeof(int))),
      entity);
  }

  private static Expression<Func<EntityWithCompositeKey, bool>> CreateCompositeKeyTemplate()
  {
    ParameterExpression entity = Expression.Parameter(typeof(EntityWithCompositeKey), nameof(entity));
    return Expression.Lambda<Func<EntityWithCompositeKey, bool>>(
      body: Expression.AndAlso(
        left: Expression.Equal(
          left: Expression.Property(
            expression: entity,
            propertyName: nameof(EntityWithCompositeKey.Id1)),
          right: Expression.Variable(typeof(int))),
        right: Expression.Equal(
          left: Expression.Property(
            expression: entity,
            propertyName: nameof(EntityWithCompositeKey.Id2)),
          right: Expression.Variable(typeof(string)))),
      entity);
  }
}

internal static class Models
{
  public static readonly IModel SimplePropertyKeyModel = CreateSimplePropertyKeyModel();
  public static readonly IModel SimpleFieldKeyModel = CreateSimpleFieldKeyModel();
  public static readonly IModel SimpleShadowKeyModel = CreateSimpleShadowKeyModel();
  public static readonly IModel CompositeKeyModel = CreateCompositeKeyModel();

  private static IModel CreateSimplePropertyKeyModel()
  {
    Type entityType = typeof(EntityWithSimplePropertyKey);
    PropertyInfo idProperty = entityType.GetRequiredProperty(nameof(EntityWithSimplePropertyKey.Id));

    IProperty idPropertyModel = Mock.Of<IProperty>(property =>
      property.Name == idProperty.Name &&
      property.ClrType == idProperty.PropertyType &&
      property.PropertyInfo == idProperty &&
      property.FieldInfo == null,
      MockBehavior.Strict);

    return CreateModel(entityType, idPropertyModel);
  }

  private static IModel CreateSimpleFieldKeyModel()
  {
    Type entityType = typeof(EntityWithSimpleFieldKey);
    FieldInfo idField = entityType.GetRequiredField(nameof(EntityWithSimpleFieldKey.Id));

    IProperty idPropertyModel = Mock.Of<IProperty>(property =>
      property.Name == idField.Name &&
      property.ClrType == idField.FieldType &&
      property.PropertyInfo == null &&
      property.FieldInfo == idField,
      MockBehavior.Strict);

    return CreateModel(entityType, idPropertyModel);
  }

  private static IModel CreateSimpleShadowKeyModel()
  {
    Type entityType = typeof(EntityWithSimpleShadowKey);

    IProperty idPropertyModel = Mock.Of<IProperty>(property =>
      property.Name == "Id" &&
      property.ClrType == typeof(int) &&
      property.PropertyInfo == null &&
      property.FieldInfo == null,
      MockBehavior.Strict);

    return CreateModel(entityType, idPropertyModel);
  }

  private static IModel CreateCompositeKeyModel()
  {
    Type entityType = typeof(EntityWithCompositeKey);

    PropertyInfo id1Property = entityType.GetRequiredProperty(nameof(EntityWithCompositeKey.Id1));
    PropertyInfo id2Property = entityType.GetRequiredProperty(nameof(EntityWithCompositeKey.Id2));

    IProperty id1PropertyModel = Mock.Of<IProperty>(property =>
      property.Name == id1Property.Name &&
      property.ClrType == id1Property.PropertyType &&
      property.PropertyInfo == id1Property &&
      property.FieldInfo == null,
      MockBehavior.Strict);

    IProperty id2PropertyModel = Mock.Of<IProperty>(property =>
      property.Name == id2Property.Name &&
      property.ClrType == id2Property.PropertyType &&
      property.PropertyInfo == id2Property &&
      property.FieldInfo == null,
      MockBehavior.Strict);

    return CreateModel(entityType, id1PropertyModel, id2PropertyModel);
  }

  private static IModel CreateModel(Type entityType, params IProperty[] propertyModels)
  {
    IEntityType entityTypeModel = Mock.Of<IEntityType>(entityType =>
      entityType.FindPrimaryKey()!.Properties == propertyModels,
      MockBehavior.Strict);

    return Mock.Of<IModel>(model =>
      model.FindEntityType(entityType) == entityTypeModel,
      MockBehavior.Strict);
  }
}
