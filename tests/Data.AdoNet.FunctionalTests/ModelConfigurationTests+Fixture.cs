namespace CodeArchitects.Platform.Data.AdoNet;

public partial class ModelConfigurationTests
{
  private static class WithAOneToManyAggregation
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

        Aggregation<Parent, Child>(aggregation => aggregation
          .OneToMany()
          .Navigation(x => x.Children)
          .UsingForeignKey(x => x.ParentId));
      }
    }
  }
}
