using CodeArchitects.Platform.Data.AdoNet;
using CodeArchitects.Platform.Data.AdoNet.DependencyInjection;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public interface IAdoNetConfigurationBuilder
{
  IAdoNetConfigurationBuilderWithProvider UseProvider<TProvider>(Action<TProvider> configureAction)
    where TProvider : DatabaseProvider, new();
}

public interface IAdoNetConfigurationBuilderWithProvider
{
  IAdoNetConfigurationBuilderWithProvider UseModel(Type modelConfigurationType);

  IAdoNetConfigurationBuilderWithProvider AddCommandInterceptor(Type interceptorType);

  IAdoNetConfigurationBuilderWithProvider ScanServicesFromAssembly(Assembly assembly, AdoNetServiceTypes serviceTypes = AdoNetServiceTypes.All);
}