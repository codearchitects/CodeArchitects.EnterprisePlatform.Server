namespace CodeArchitects.Platform.Data.AdoNet;

public partial class ModelConfigurationTests
{
  private static class WithAOneToManyIntraAggregate
  {
    public class Parent
    {
      public int Id { get; set; }
      public string? Name { get; set; }
      public ICollection<Child>? Children { get; set; }
    }

    public class Child
    {
      public int Id { get; set; }
      public string? Name { get; set; }
      public int ParentId { get; set; }
    }

    public class TestModelConfiguration : ModelConfiguration
    {
      protected override void Configure()
      {
        Entity<Parent>(entity =>
        {
          entity.WithTableName("Entities");
        });

        Entity<Child>();

        Aggregate<Parent, Child>(aggregate => aggregate
          .OneToMany()
          .Navigation(x => x.Children)
          .UsingForeignKey(x => x.ParentId));
      }
    }
  }

  private static class WithAManyToManyInterAggregate
  {
    public class EntityA
    {
      public int Id { get; set; }
      public string? Name { get; set; }
      public ICollection<EntityB>? Others { get; set; }
    }

    public class EntityB
    {
      public int Id { get; set; }
      public string? Name { get; set; }
      public ICollection<EntityA>? Others { get; set; }
    }

    public class TestModelConfiguration : ModelConfiguration
    {
      protected override void Configure()
      {
        Entity<EntityA>();

        Entity<EntityB>();

        Associate<EntityA, EntityB>(associate => associate
          .ManyToMany()
          .Navigation(x => x.Others)
          .InverseNavigation(x => x.Others));
      }
    }
  }
}
