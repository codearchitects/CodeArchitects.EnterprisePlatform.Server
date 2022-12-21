using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore5.DependencyInjection;

internal class EntityFrameworkCoreConfigurationBuilder : IEntityFrameworkCoreConfigurationBuilder
{
  public Type? SeedType { get; private set; }

  public IEntityFrameworkCoreConfigurationBuilder UseSeed(Type seedType)
  {
    if (seedType is null)
      throw new ArgumentNullException(nameof(seedType));

    if (!seedType.IsSubclassOf(typeof(DataSeed)))
      throw new ArgumentException($"Type '{seedType}' does not extend '{nameof(DataSeed)}'.");

    SeedType = seedType;
    return this;
  }
}
