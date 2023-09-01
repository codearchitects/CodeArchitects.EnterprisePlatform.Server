using CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures.Model;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Fixtures;

public class TestDataSeed : DataSeed
{
  private readonly TenantEntity _entity;

  public TestDataSeed(TenantEntity entity)
  {
    _entity = entity;
  }

  public override void Seed(ISeeder seeder)
  {
    seeder.Seed(_entity);
  }
}