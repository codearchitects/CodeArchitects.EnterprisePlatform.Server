using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using CodeArchitects.Platform.Infrastructure.Dapr.Messaging;
using CodeArchitects.Platform.Infrastructure.Dapr.State;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

public class DaprConfigurationBuilderTests
{
  private readonly DaprConfigurationBuilder _sut;

  public DaprConfigurationBuilderTests()
  {
    _sut = new DaprConfigurationBuilder();
  }

  [Fact]
  public void Build_ShouldYieldEmptyConfiguration_WhenCalledAlone()
  {
    // Arrange
    DaprConfiguration expectedConfiguration = new DaprConfiguration();

    // Act
    DaprConfiguration configuration = _sut.Build();

    // Assert
    configuration.Should().BeEquivalentTo(expectedConfiguration);
  }

  [Fact]
  public void AddServiceOptions_WithConfiguration_ShouldAddServiceOptions()
  {
    // Arrange
    DaprConfiguration expectedConfiguration = new DaprConfiguration
    {
      Service = new ServiceOptions
      {
        Messaging = new DaprMessagingOptions
        {
          DefaultBus = "DefaultBus",
          Bindings = new Dictionary<string, DaprMessagingBindings>
          {
            ["Message1Handler"] = new DaprMessagingBindings
            {
              BusName = "BusName1",
              Topic = "Topic1"
            },
            ["Message2Handler"] = new DaprMessagingBindings
            {
              BusName = "BusName2",
              Topic = "Topic2"
            }
          }
        },
        State = new DaprStateOptions
        {
          DefaultStore = "DefaultStore"
        }
      }
    };
    ConfigurationSectionStub section = new ConfigurationSectionStub(null, null, new
    {
      Caep = new
      {
        Dapr = new
        {
          Messaging = new
          {
            DefaultBus = "DefaultBus",
            Bindings = new
            {
              Message1Handler = new
              {
                BusName = "BusName1",
                Topic = "Topic1"
              },
              Message2Handler = new
              {
                BusName = "BusName2",
                Topic = "Topic2"
              }
            }
          },
          State = new
          {
            DefaultStore = "DefaultStore"
          }
        }
      }
    });

    // Act
    _sut.AddServiceOptions(section as IConfiguration);
    DaprConfiguration configuration = _sut.Build();

    // Assert
    configuration.Should().BeEquivalentTo(expectedConfiguration);
  }

  [Fact]
  public void AddServiceOptions_WithConfigurationSection_ShouldAddServiceOptions()
  {
    // Arrange
    DaprConfiguration expectedConfiguration = new DaprConfiguration
    {
      Service = new ServiceOptions
      {
        Messaging = new DaprMessagingOptions
        {
          DefaultBus = "DefaultBus"
        },
        State = new DaprStateOptions
        {
          DefaultStore = "DefaultStore"
        }
      }
    };
    ConfigurationSectionStub section = new ConfigurationSectionStub(null, null, expectedConfiguration.Service);

    // Act
    _sut.AddServiceOptions(section as IConfigurationSection);
    DaprConfiguration configuration = _sut.Build();

    // Assert
    configuration.Should().BeEquivalentTo(expectedConfiguration);
  }

  private class ConfigurationSectionStub : IConfigurationSection
  {
    private readonly Dictionary<string, ConfigurationSectionStub>? _sections;

    public ConfigurationSectionStub(string? path, string? key, object instance)
    {
      if (path is not null && key is null)
        throw new InvalidOperationException($"'{nameof(path)}' is not null but '{nameof(key)}' is null");

      Key = key!;
      Path = string.IsNullOrEmpty(path) ? Key : $"{path}.{Key}";
      Type instanceType = instance.GetType();
      if (instanceType.IsValueType || instanceType == typeof(string))
      {
        Value = instance.ToString()!;
      }
      else
      {
        PropertyInfo[] properties = instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        _sections = new Dictionary<string, ConfigurationSectionStub>(properties
          .Select(x => KeyValuePair.Create(x.Name, x.GetValue(instance)))
          .Where(x => x.Value is not null)
          .Select(x => KeyValuePair.Create(x.Key, new ConfigurationSectionStub(Path, x.Key, x.Value!))));
        Value = default!;
      }
    }

    public string this[string key]
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public string Key { get; }

    public string Path { get; }

    public string Value { get; set; }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
      return _sections?.Values ?? Enumerable.Empty<IConfigurationSection>();
    }

    public IChangeToken GetReloadToken()
    {
      throw new NotImplementedException();
    }

    public IConfigurationSection GetSection(string key)
    {
      return GetSection(key.Split(":"));
    }

    private IConfigurationSection GetSection(string[] segments)
    {
      if (segments.Length == 1)
      {
        return _sections?.GetValueOrDefault(segments[0]) ?? null!;
      }
      return GetSection(segments[..^1])?.GetSection(segments[^1]) ?? null!;
    }
  }
}
