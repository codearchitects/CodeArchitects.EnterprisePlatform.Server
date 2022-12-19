using CodeArchitects.Platform.Data.AdoNet;
using EFCoreSample.Domain.Model;

namespace EFCoreSample.Infrastructure.Data;

public class DataConfiguration : ModelConfiguration
{
  protected override void Configure()
  {
    Entity<Cart>();
  }
}
