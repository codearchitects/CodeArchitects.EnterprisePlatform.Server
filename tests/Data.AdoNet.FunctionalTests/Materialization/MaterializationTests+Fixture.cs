using Bogus;
using CodeArchitects.Platform.Data.AdoNet.Fixtures;
using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Model.FluentMock;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Materialization;

public partial class MaterializationTests
{
  public class Parent
  {
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public List<ChildA>? ChildrenA { get; set; }
    public HashSet<ChildB>? ChildrenB { get; set; }
    public ChildC? ChildC { get; set; }

    private static readonly Faker<Parent> s_faker = new Faker<Parent>()
      .RuleFor(parent => parent.Id, Guid.NewGuid)
      .RuleFor(parent => parent.Name, faker => faker.Name.FullName());

    public static Parent One() => s_faker.Generate();
  }

  public class ChildA
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public Guid ParentId { get; set; }

    public List<ChildD>? ChildrenD { get; set; }

    private static readonly Faker<ChildA> s_faker = new Faker<ChildA>()
      .RuleFor(childA => childA.Id, faker => faker.Random.Int())
      .RuleFor(childA => childA.Name, faker => faker.Name.FullName());

    public static ChildA One(Guid parentId)
    {
      ChildA childA = s_faker.Generate();
      childA.ParentId = parentId;
      return childA;
    }

    public static List<ChildA> Many(int count, Guid parentId)
    {
      var list = s_faker.Generate(count);
      list.ForEach(item => item.ParentId = parentId);
      return list;
    }
  }

  public class ChildB
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public Guid ParentId { get; set; }

    private static readonly Faker<ChildB> s_faker = new Faker<ChildB>()
      .RuleFor(childB => childB.Id, faker => faker.Random.Int())
      .RuleFor(childB => childB.Name, faker => faker.Name.FullName());

    public static ChildB One(Guid parentId)
    {
      ChildB childB = s_faker.Generate();
      childB.ParentId = parentId;
      return childB;
    }

    public static List<ChildB> Many(int count, Guid parentId)
    {
      var list = s_faker.Generate(count);
      list.ForEach(item => item.ParentId = parentId);
      return list;
    }
  }

  public class ChildC
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public Guid ParentId { get; set; }

    private static readonly Faker<ChildC> s_faker = new Faker<ChildC>()
      .RuleFor(childC => childC.Id, faker => faker.Random.Int())
      .RuleFor(childC => childC.Name, faker => faker.Name.FullName());

    public static ChildC One(Guid parentId)
    {
      ChildC childC = s_faker.Generate();
      childC.ParentId = parentId;
      return childC;
    }

    public static List<ChildC> Many(int count, Guid parentId)
    {
      var list = s_faker.Generate(count);
      list.ForEach(item => item.ParentId = parentId);
      return list;
    }
  }

  public class ChildD
  {
    public ChildD(int id)
    {
      Id = id;
    }

    public int Id { get; set; }
    public string? Name { get; set; }
    public int ChildAId { get; set; }

    private static readonly Faker<ChildD> s_faker = new Faker<ChildD>()
      .CustomInstantiator(faker => new ChildD(faker.Random.Int()))
      .RuleFor(childC => childC.Name, faker => faker.Name.FullName());

    public static ChildD One() => s_faker.Generate();

    public static ChildD One(int childAId)
    {
      ChildD childC = s_faker.Generate();
      childC.ChildAId = childAId;
      return childC;
    }

    public static List<ChildD> Many(int count, int childAId)
    {
      var list = s_faker.Generate(count);
      list.ForEach(item => item.ChildAId = childAId);
      return list;
    }
  }

  public static class Model
  {
    public static readonly IEntityModel<Parent, Guid> ParentModel = CreateParentModel();
    public static readonly IEntityModel<ChildA, int> ChildAModel = CreateChildAModel();
    public static readonly IEntityModel<ChildB, int> ChildBModel = CreateChildBModel();
    public static readonly IEntityModel<ChildC, int> ChildCModel = CreateChildCModel();
    public static readonly IEntityModel<ChildD, int> ChildDModel = CreateChildDModel();

    public static readonly IAccessibleSimpleNavigationModel ParentToChildANavigation = CreateParentToChildANavigation();
    public static readonly IAccessibleSimpleNavigationModel ParentToChildBNavigation = CreateParentToChildBNavigation();
    public static readonly IAccessibleSimpleNavigationModel ParentToChildCNavigation = CreateParentToChildCNavigation();
    public static readonly IAccessibleSimpleNavigationModel ChildAToChildDNavigation = CreateChildAToChildDNavigation();

    private static IEntityModel<Parent, Guid> CreateParentModel()
    {
      PropertyInfo idPropertyInfo = typeof(Parent).GetRequiredProperty(nameof(Parent.Id));
      PropertyInfo namePropertyInfo = typeof(Parent).GetRequiredProperty(nameof(Parent.Name));

      IStandardPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetIndex(0)
        .SetType(typeof(Guid))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(idPropertyInfo)));
      IOrdinaryColumnModel nameProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(1)
        .SetType(typeof(string))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(namePropertyInfo)));

      IEntityModel entity = EntityModelBuilder.Build(_ => _
        .SetType(typeof(Parent))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty)
          .SetIsComposite(false)
          .SetType(typeof(Guid)))
        .SetColumns(idProperty, nameProperty)
        .SetInitializer(_ => _
          .SetConstructor(typeof(Parent).GetRequiredConstructor())
          .SetConstructorProperties()
          .SetInitializerProperties(idProperty, nameProperty)));

      return entity.Mocked<Parent, Guid>();
    }

    private static IEntityModel<ChildA, int> CreateChildAModel()
    {
      PropertyInfo idPropertyInfo = typeof(ChildA).GetRequiredProperty(nameof(ChildA.Id));
      PropertyInfo namePropertyInfo = typeof(ChildA).GetRequiredProperty(nameof(ChildA.Name));
      PropertyInfo parentIdPropertyInfo = typeof(ChildA).GetRequiredProperty(nameof(ChildA.ParentId));

      IStandardPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetIndex(0)
        .SetType(typeof(int))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(idPropertyInfo)));
      IOrdinaryColumnModel nameProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(1)
        .SetType(typeof(string))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(namePropertyInfo)));
      IOrdinaryColumnModel parentIdProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(2)
        .SetType(typeof(Guid))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(parentIdPropertyInfo)));

      IEntityModel entity = EntityModelBuilder.Build(_ => _
        .SetType(typeof(ChildA))
        .SetPrimaryKey(PrimaryKeyModelBuilder.Build(_ => _
          .SetColumns(idProperty)
          .SetIsComposite(false)
          .SetType(typeof(int))
          .Setup(mock => mock
            .Setup(x => x.GetValue)
            .Returns(idPropertyInfo.GetValue))))
        .SetColumns(idProperty, nameProperty, parentIdProperty)
        .SetInitializer(_ => _
          .SetConstructor(typeof(ChildA).GetRequiredConstructor())
          .SetConstructorProperties()
          .SetInitializerProperties(idProperty, nameProperty, parentIdProperty)));

      return entity.Mocked<ChildA, int>();
    }

    private static IEntityModel<ChildB, int> CreateChildBModel()
    {
      PropertyInfo idPropertyInfo = typeof(ChildB).GetRequiredProperty(nameof(ChildB.Id));
      PropertyInfo namePropertyInfo = typeof(ChildB).GetRequiredProperty(nameof(ChildB.Name));
      PropertyInfo parentIdPropertyInfo = typeof(ChildB).GetRequiredProperty(nameof(ChildB.ParentId));

      IStandardPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetIndex(0)
        .SetType(typeof(int))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(idPropertyInfo)));
      IOrdinaryColumnModel nameProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(1)
        .SetType(typeof(string))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(namePropertyInfo)));
      IOrdinaryColumnModel parentIdProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(2)
        .SetType(typeof(Guid))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(parentIdPropertyInfo)));

      IEntityModel entity = EntityModelBuilder.Build(_ => _
        .SetType(typeof(ChildB))
        .SetPrimaryKey(PrimaryKeyModelBuilder.Build(_ => _
          .SetColumns(idProperty)
          .SetIsComposite(false)
          .SetType(typeof(int))
          .Setup(mock => mock
            .Setup(x => x.GetValue)
            .Returns(idPropertyInfo.GetValue))))
        .SetColumns(idProperty, nameProperty, parentIdProperty)
        .SetInitializer(_ => _
          .SetConstructor(typeof(ChildB).GetRequiredConstructor())
          .SetConstructorProperties()
          .SetInitializerProperties(idProperty, nameProperty, parentIdProperty)));

      return entity.Mocked<ChildB, int>();
    }

    private static IEntityModel<ChildC, int> CreateChildCModel()
    {
      PropertyInfo idPropertyInfo = typeof(ChildC).GetRequiredProperty(nameof(ChildC.Id));
      PropertyInfo namePropertyInfo = typeof(ChildC).GetRequiredProperty(nameof(ChildC.Name));
      PropertyInfo parentIdPropertyInfo = typeof(ChildC).GetRequiredProperty(nameof(ChildC.ParentId));

      IStandardPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetIndex(0)
        .SetType(typeof(int))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(idPropertyInfo)));
      IOrdinaryColumnModel nameProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(1)
        .SetType(typeof(string))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(namePropertyInfo)));
      IOrdinaryColumnModel parentIdProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(2)
        .SetType(typeof(Guid))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(parentIdPropertyInfo)));

      IEntityModel entity = EntityModelBuilder.Build(_ => _
        .SetType(typeof(ChildC))
        .SetPrimaryKey(_ => _
          .SetColumns(idProperty)
          .SetIsComposite(false)
          .SetType(typeof(int)))
        .SetColumns(idProperty, nameProperty, parentIdProperty)
        .SetInitializer(_ => _
          .SetConstructor(typeof(ChildC).GetRequiredConstructor())
          .SetConstructorProperties()
          .SetInitializerProperties(idProperty, nameProperty, parentIdProperty)));

      return entity.Mocked<ChildC, int>();
    }

    private static IEntityModel<ChildD, int> CreateChildDModel()
    {
      PropertyInfo idPropertyInfo = typeof(ChildD).GetRequiredProperty(nameof(ChildD.Id));
      PropertyInfo namePropertyInfo = typeof(ChildD).GetRequiredProperty(nameof(ChildD.Name));
      PropertyInfo childAIdPropertyInfo = typeof(ChildD).GetRequiredProperty(nameof(ChildD.ChildAId));

      IStandardPrimaryKeyColumnModel idProperty = PrimaryKeyColumnModelBuilder.Build(_ => _
        .SetIndex(0)
        .SetType(typeof(int))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(idPropertyInfo)));
      IOrdinaryColumnModel nameProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(1)
        .SetType(typeof(string))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(namePropertyInfo)));
      IOrdinaryColumnModel childAIdProperty = OrdinaryColumnModelBuilder.Build(_ => _
        .SetIndex(2)
        .SetType(typeof(int))
        .Setup(mock => mock
          .Setup(x => x.Member)
          .Returns(childAIdPropertyInfo)));

      IEntityModel entity = EntityModelBuilder.Build(_ => _
        .SetType(typeof(ChildD))
        .SetPrimaryKey(PrimaryKeyModelBuilder.Build(_ => _
          .SetColumns(idProperty)
          .SetIsComposite(false)
          .SetType(typeof(int))
          .Setup(mock => mock
            .Setup(x => x.GetValue)
            .Returns(idPropertyInfo.GetValue))))
        .SetColumns(idProperty, nameProperty, childAIdProperty)
        .SetInitializer(_ => _
          .SetConstructor(typeof(ChildD).GetRequiredConstructor())
          .SetConstructorProperties(idProperty)
          .SetInitializerProperties(nameProperty, childAIdProperty)));

      return entity.Mocked<ChildD, int>();
    }

    private static IAccessibleSimpleNavigationModel CreateParentToChildANavigation()
    {
      PropertyInfo propertyInfo = typeof(Parent).GetRequiredProperty(nameof(Parent.ChildrenA));

      return SimpleAccessibleNavigationModelBuilder.Build(_ => _
        .SetFrom(ParentModel)
        .SetTo(ChildAModel)
        .SetIsCollection(true)
        .SetCollectionKind(CollectionKind.List)
        .Setup(mock => mock
          .Setup(x => x.GetValue)
          .Returns(propertyInfo.GetValue))
        .Setup(mock => mock
          .Setup(x => x.SetValue)
          .Returns(propertyInfo.SetValue)));
    }

    private static IAccessibleSimpleNavigationModel CreateParentToChildBNavigation()
    {
      PropertyInfo propertyInfo = typeof(Parent).GetRequiredProperty(nameof(Parent.ChildrenB));

      return SimpleAccessibleNavigationModelBuilder.Build(_ => _
        .SetFrom(ParentModel)
        .SetTo(ChildBModel)
        .SetIsCollection(true)
        .SetCollectionKind(CollectionKind.HashSet)
        .Setup(mock => mock
          .Setup(x => x.GetValue)
          .Returns(propertyInfo.GetValue))
        .Setup(mock => mock
          .Setup(x => x.SetValue)
          .Returns(propertyInfo.SetValue)));
    }

    private static IAccessibleSimpleNavigationModel CreateParentToChildCNavigation()
    {
      PropertyInfo propertyInfo = typeof(Parent).GetRequiredProperty(nameof(Parent.ChildC));

      return SimpleAccessibleNavigationModelBuilder.Build(_ => _
        .SetFrom(ParentModel)
        .SetTo(ChildCModel)
        .SetIsCollection(false)
        .Setup(mock => mock
          .Setup(x => x.GetValue)
          .Returns(propertyInfo.GetValue))
        .Setup(mock => mock
          .Setup(x => x.SetValue)
          .Returns(propertyInfo.SetValue)));
    }

    private static IAccessibleSimpleNavigationModel CreateChildAToChildDNavigation()
    {
      PropertyInfo propertyInfo = typeof(ChildA).GetRequiredProperty(nameof(ChildA.ChildrenD));

      return SimpleAccessibleNavigationModelBuilder.Build(_ => _
        .SetFrom(ChildAModel)
        .SetTo(ChildDModel)
        .SetIsCollection(true)
        .SetCollectionKind(CollectionKind.List)
        .Setup(mock => mock
          .Setup(x => x.GetValue)
          .Returns(propertyInfo.GetValue))
        .Setup(mock => mock
          .Setup(x => x.SetValue)
          .Returns(propertyInfo.SetValue)));
    }
  }
}
