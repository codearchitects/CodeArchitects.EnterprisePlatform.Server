using CodeArchitects.Platform.Data.EntityFrameworkCore.Helpers;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Concurrency;

internal class ConventionSetPlugin : IConventionSetPlugin
{
  public ConventionSet ModifyConventions(ConventionSet conventionSet)
  {
    if (!EFCoreEnvironment.IsDesignTime)
    {
      conventionSet.ModelFinalizingConventions.Add(new Convention());
    }

    return conventionSet;
  }

  private sealed class Convention : IModelFinalizingConvention
  {
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
      foreach (IConventionEntityType entityType in modelBuilder.Metadata.GetEntityTypes())
      {
        IConventionProperty? concurrencyToken = entityType.GetProperties().FirstOrDefault(property => property.IsConcurrencyToken);
        if (concurrencyToken is null)
          continue;

        entityType.HasConcurrencyCheck(concurrencyToken);
      }
    }
  }
}
