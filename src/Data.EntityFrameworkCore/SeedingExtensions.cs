using CodeArchitects.Platform.Common.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore
{
  public static class SeedingExtensions
  {
    public static void Seed<TDataSeed>(this IServiceScope scope)
      where TDataSeed : DataSeed
    {
      IServiceProvider services = scope.ServiceProvider;

      Type contextType =
        typeof(TDataSeed).GetCustomAttribute<SeedingContextAttribute>()?.ContextType ??
        typeof(TDataSeed).Assembly.GetTypes().SingleOrDefault(x => x.IsAssignableTo(typeof(DbContext))) ??
        typeof(DbContext);

      DbContext context = (DbContext)services.GetRequiredService(contextType);
      Seeder seeder = new Seeder(context);
      TDataSeed dataSeed = CreationHelpers.CreateFromServices<TDataSeed>(services);

      dataSeed.Init(seeder);
      context.SaveChanges();
    }

    public static void Seed<TDataSeed>(this DbContext context, IServiceProvider services)
      where TDataSeed : DataSeed
    {
      Seeder seeder = new Seeder(context);
      TDataSeed dataSeed = CreationHelpers.CreateFromServices<TDataSeed>(services);

      dataSeed.Init(seeder);
      context.SaveChanges();
    }
  }
}
