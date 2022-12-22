using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

internal abstract class CaepExtensionBase : IDbContextOptionsExtension
{
  public abstract DbContextOptionsExtensionInfo Info { get; }

  public virtual void ApplyServices(IServiceCollection services)
  {
  }

  public virtual void Validate(IDbContextOptions options)
  {
  }
}
