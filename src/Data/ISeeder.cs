using System.Collections.Generic;

namespace CodeArchitects.Platform.Data;

public interface ISeeder
{
  void Seed<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
}

public static class SeederExtensions
{
  public static void Seed<TEntity>(this ISeeder seeder, params TEntity[] entities) where TEntity : class
    => seeder.Seed(entities);
}
