namespace CodeArchitects.Platform.Dapr.AspNetCore.Configuration;

public static class DaprConfigurationBuilderExtensions
{
  public static TSection GetOrAdd<TSection>(this IDaprConfigurationBuilder builder, string key)
    where TSection : class, new()
  {
    if (builder.Configuration.GetSection<TSection>() is not TSection section)
    {
      section = builder.AddSection<TSection>(key);
    }

    return section;
  }

  public static TSection GetOrAdd<TSection>(this IDaprConfigurationBuilder builder, string key, Action<TSection> configure)
    where TSection : class, new()
  {
    if (builder.Configuration.GetSection<TSection>() is not TSection section)
    {
      section = builder.AddSection<TSection>(key);
      configure(section);
    }

    return section;
  }

  public static TSection GetOrAdd<TSection>(this IDaprConfigurationBuilder builder, string key, Action<TSection, IDaprConfiguration> configure)
    where TSection : class, new()
  {
    if (builder.Configuration.GetSection<TSection>() is not TSection section)
    {
      section = builder.AddSection<TSection>(key);
      configure(section, builder.Configuration);
    }

    return section;
  }
}
