using Bogus;
using CodeArchitects.Platform.Data.AdoNet.Model;

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

  public class Model : ModelConfiguration
  {
    public static readonly IEntityModel<Parent, Guid> ParentModel;
    public static readonly IEntityModel<ChildA, int> ChildAModel;
    public static readonly IEntityModel<ChildB, int> ChildBModel;
    public static readonly IEntityModel<ChildC, int> ChildCModel;
    public static readonly IEntityModel<ChildD, int> ChildDModel;

    public static readonly IAccessibleSimpleNavigationModel ParentToChildANavigation;
    public static readonly IAccessibleSimpleNavigationModel ParentToChildBNavigation;
    public static readonly IAccessibleSimpleNavigationModel ParentToChildCNavigation;
    public static readonly IAccessibleSimpleNavigationModel ChildAToChildDNavigation;

    static Model()
    {
      IDataModel dataModel = new Model().CreateDataModel();

      ParentModel = dataModel.GetEntity<Parent, Guid>();
      ChildAModel = dataModel.GetEntity<ChildA, int>();
      ChildBModel = dataModel.GetEntity<ChildB, int>();
      ChildCModel = dataModel.GetEntity<ChildC, int>();
      ChildDModel = dataModel.GetEntity<ChildD, int>();

      ParentToChildANavigation = (IAccessibleSimpleNavigationModel)ParentModel.GetNavigation(nameof(Parent.ChildrenA));
      ParentToChildBNavigation = (IAccessibleSimpleNavigationModel)ParentModel.GetNavigation(nameof(Parent.ChildrenB));
      ParentToChildCNavigation = (IAccessibleSimpleNavigationModel)ParentModel.GetNavigation(nameof(Parent.ChildC));
      ChildAToChildDNavigation = (IAccessibleSimpleNavigationModel)ChildAModel.GetNavigation(nameof(ChildA.ChildrenD));
    }

    private Model() { }

    protected override void Configure()
    {
      Entity<Parent>();
      Entity<ChildA>();
      Entity<ChildB>();
      Entity<ChildC>();
      Entity<ChildD>();

      Associate<Parent, ChildA>(associate => associate
        .OneToMany()
        .Navigation(parent => parent.ChildrenA)
        .UsingForeignKey(childA => childA.ParentId));

      Associate<Parent, ChildB>(associate => associate
        .OneToMany()
        .Navigation(parent => parent.ChildrenB)
        .UsingForeignKey(childB => childB.ParentId));

      Associate<Parent, ChildC>(associate => associate
        .OneToOne()
        .Navigation(parent => parent.ChildC)
        .UsingForeignKey(childC => childC.ParentId));

      Associate<ChildA, ChildD>(associate => associate
        .OneToMany()
        .Navigation(childA => childA.ChildrenD)
        .UsingForeignKey(childD => childD.ChildAId));
    }
  }
}
