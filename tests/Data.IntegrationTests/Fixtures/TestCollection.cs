namespace CodeArchitects.Platform.Data.Fixtures;

[CollectionDefinition(Name)]
public class TestCollection : ICollectionFixture<TestFixture>
{
  public const string Name = nameof(TestCollection);
}
