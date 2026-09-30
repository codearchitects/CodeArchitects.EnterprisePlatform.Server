using CodeArchitects.Platform.Data.Mapster;
using Mapster;
using Mapster.Models;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Methods for adding the Mapster mapping used by the mapped repositories to the application services.
/// </summary>
public static class DataMapsterServiceCollectionExtensions
{
  /// <summary>
  /// Registers a dedicated, strict and eagerly validated Mapster configuration for the data layer,
  /// together with the <see cref="IDataMapper"/> that uses it.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configure">Adds the mapping rules to the configuration.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="InvalidOperationException">The mapping configuration is invalid.</exception>
  public static IServiceCollection AddDataMapster(this IServiceCollection services, Action<TypeAdapterConfig> configure)
  {
    ArgumentNullException.ThrowIfNull(services);
    ArgumentNullException.ThrowIfNull(configure);

    TypeAdapterConfig config = new()
    {
      RequireExplicitMapping = true,
      RequireDestinationMemberSource = true
    };

    if (services.Any(descriptor => descriptor.ServiceType == typeof(DataMapsterConfiguration)))
      throw new InvalidOperationException(
        "AddDataMapster was already called: the data layer has a single mapping configuration. Register every rule in one call, passing all the assemblies to scan or a single configure action.");

    configure(config);
    CheckReversedRules(config);
    Compile(config);

    services.AddSingleton(new DataMapsterConfiguration(config));
    services.AddScoped<IDataMapper>(sp => new DataMapper(sp, sp.GetRequiredService<DataMapsterConfiguration>().Config));

    return services;
  }

  /// <summary>
  /// Registers a dedicated, strict and eagerly validated Mapster configuration for the data layer, with the
  /// rules of every <see cref="IRegister"/> found in <paramref name="assemblies"/>.
  /// </summary>
  /// <remarks>
  /// See <see cref="AddDataMapster(IServiceCollection, Action{TypeAdapterConfig})"/>.
  /// </remarks>
  /// <param name="services">The service collection.</param>
  /// <param name="assemblies">The assemblies to scan for <see cref="IRegister"/> implementations.</param>
  /// <returns>The same <see cref="IServiceCollection"/> instance.</returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentException"><paramref name="assemblies"/> is empty.</exception>
  /// <exception cref="InvalidOperationException">The mapping configuration is invalid.</exception>
  public static IServiceCollection AddDataMapster(this IServiceCollection services, params Assembly[] assemblies)
  {
    ArgumentNullException.ThrowIfNull(assemblies);
    if (assemblies.Length == 0)
      throw new ArgumentException("At least one assembly to scan for IRegister implementations is required.", nameof(assemblies));

    return services.AddDataMapster(config => config.Scan(assemblies));
  }

  private static void CheckReversedRules(TypeAdapterConfig config)
  {
    if (!config.RequireDestinationMemberSource)
      return;

    foreach (KeyValuePair<TypeTuple, TypeAdapterRule> rule in config.RuleMap)
    {
      TypeAdapterSettings settings = rule.Value.Settings;
      bool reversed = settings.SkipDestinationMemberCheck == true
        && settings.Unflattening == true
        && config.RuleMap.ContainsKey(new TypeTuple(rule.Key.Destination, rule.Key.Source));

      if (reversed)
      {
        settings.SkipDestinationMemberCheck = false;
      }
    }
  }

  private static void Compile(TypeAdapterConfig config)
  {
    try
    {
      config.Compile();
    }
    catch (Exception exception)
    {
      throw new InvalidOperationException(
        $"The Mapster data mapping configuration is invalid: {Describe(exception)}", exception);
    }
  }

  private static string Describe(Exception exception)
  {
    List<string> messages = [];
    for (Exception? current = exception; current is not null; current = current.InnerException)
    {
      messages.Add(current.Message);
    }

    return string.Join(" ", messages);
  }
}
