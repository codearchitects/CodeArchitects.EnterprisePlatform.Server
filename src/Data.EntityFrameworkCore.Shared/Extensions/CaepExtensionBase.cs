using CodeArchitects.Platform.Data.EntityFrameworkCore.Features;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal abstract class CaepExtensionBase : IDbContextOptionsExtension
{
  public abstract DbContextOptionsExtensionInfo Info { get; }

  public virtual void ApplyServices(IServiceCollection services)
  {
    services.AddSingleton<IRelationalTypeMappingSourcePlugin, RuntimeAnnotationTypeMappingSourcePlugin>();
  }

  public virtual void Validate(IDbContextOptions options)
  {
  }
}
