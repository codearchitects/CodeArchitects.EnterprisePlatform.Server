using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Extensions;

/// <summary>
/// Interface that a plug-in for the CAEP extension must implement.
/// </summary>
public interface ICaepExtensionPlugin
{
  /// <inheritdoc cref="IDbContextOptionsExtension.ApplyServices(IServiceCollection)"/>
  void ApplyServices(IPluginServiceCollection services);
}
