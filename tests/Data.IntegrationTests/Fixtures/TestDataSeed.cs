namespace CodeArchitects.Platform.Data.Fixtures;

internal class TestDataSeed : DataSeed
{
  private readonly Action<ISeeder> _seedingAction;

  public TestDataSeed(Action<ISeeder> seedingAction)
  {
    _seedingAction = seedingAction;
  }

  public override void Seed(ISeeder seeder)
  {
    _seedingAction(seeder);
  }
}
