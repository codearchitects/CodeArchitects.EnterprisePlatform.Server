namespace CodeArchitects.Platform.Data;

/// <summary>
/// Extend this class to seed the database.
/// </summary>
public abstract class DataSeed
{
  /// <summary>
  /// Applies the data seed to the database.
  /// </summary>
  /// <param name="seeder">An <see cref="ISeeder"/> that can be used to add entities to the seed.</param>
  public abstract void Seed(ISeeder seeder);
}
