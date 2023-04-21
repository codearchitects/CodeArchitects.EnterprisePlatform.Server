namespace CodeArchitects.Platform.Data.MongoDB.Fixtures;

[CollectionDefinition(Name)]
public class TestCollection : ICollectionFixture<TestFixture>
{
  public const string Name = nameof(TestCollection);
}
