using CodeArchitects.Platform.Data.MongoDB.Fixtures;
using CodeArchitects.Platform.Data.Mapster;
using CodeArchitects.Platform.Data.MongoDB.Mapster;
using global::Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CodeArchitects.Platform.Data.MongoDB.Mapping;

public class MapsterMappingTests
{
  /// <summary>Every rule the Widget pair needs in strict mode.</summary>
  private static void ConfigureWidget(TypeAdapterConfig config)
  {
    config.NewConfig<Widget, WidgetDocument>()
      .TwoWays()
      .Map(document => document.Label, widget => widget.Name);
    config.NewConfig<WidgetPart, WidgetPartDocument>().TwoWays();
  }

  private static IDataMapper CreateMapper(Action<TypeAdapterConfig> configure) => new ServiceCollection()
    .AddDataMapster(configure)
    .BuildServiceProvider()
    .CreateScope().ServiceProvider.GetRequiredService<IDataMapper>();

  #region AddDataMapster

  [Fact]
  public void AddDataMapster_ShouldRegisterADedicatedMapper_AndNothingOfTheApplication()
  {
    // Arrange
    ServiceProvider provider = new ServiceCollection()
      .AddDataMapster(ConfigureWidget)
      .BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

    // Act
    using IServiceScope scope = provider.CreateScope();
    IDataMapper mapper = scope.ServiceProvider.GetRequiredService<IDataMapper>();

    // Assert
    mapper.Config.Should().NotBeSameAs(TypeAdapterConfig.GlobalSettings);
    mapper.Config.RequireExplicitMapping.Should().BeTrue();
    mapper.Config.RequireDestinationMemberSource.Should().BeTrue();
    scope.ServiceProvider.GetService<IMapper>().Should().BeNull();
    provider.GetService<TypeAdapterConfig>().Should().BeNull();
  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void AddDataMapster_ShouldNotReplaceTheApplicationMapper(bool dataFirst)
  {
    // Arrange
    // The application maps its DTOs by convention, with Mapster's own registration.
    IServiceCollection services = new ServiceCollection();
    if (dataFirst)
    {
      services.AddDataMapster(ConfigureWidget);
      services.AddMapster();
    }
    else
    {
      services.AddMapster();
      services.AddDataMapster(ConfigureWidget);
    }

    // Act
    using IServiceScope scope = services.BuildServiceProvider().CreateScope();
    IMapper applicationMapper = scope.ServiceProvider.GetRequiredService<IMapper>();
    IDataMapper dataMapper = scope.ServiceProvider.GetRequiredService<IDataMapper>();

    // Assert
    applicationMapper.Config.Should().BeSameAs(TypeAdapterConfig.GlobalSettings);
    dataMapper.Config.Should().NotBeSameAs(applicationMapper.Config);
    dataMapper.Map<Widget, WidgetDocument>(new Widget { Name = "name" }).Label.Should().Be("name");
  }

  [Fact]
  public void AddDataMapster_ShouldThrow_WhenCalledTwice()
  {
    // Arrange
    IServiceCollection services = new ServiceCollection().AddDataMapster(ConfigureWidget);

    // Act
    // A second call would silently drop the rules of the first one.
    Action act = () => services.AddDataMapster(ConfigureWidget);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("AddDataMapster was already called*");
  }

  [Fact]
  public void AddDataMapster_ShouldKeepADeliberatelySkippedCheck()
  {
    // Act
    // A partially mapped legacy document: the check is skipped on purpose, not by TwoWays().
    Action act = () => new ServiceCollection().AddDataMapster(config =>
    {
      config.NewConfig<Widget, WidgetDocument>().Settings.SkipDestinationMemberCheck = true;
      config.NewConfig<WidgetPart, WidgetPartDocument>();
    });

    // Assert
    act.Should().NotThrow();
  }

  [Fact]
  public void AddDataMapster_ShouldThrow_WhenANestedRuleIsMissing()
  {
    // Act
    Action act = () => new ServiceCollection().AddDataMapster(config => config
      .NewConfig<Widget, WidgetDocument>()
      .Map(document => document.Label, widget => widget.Name));

    // Assert
    // The pair is only reached through Widget.Part, and it still fails at registration.
    act.Should().Throw<InvalidOperationException>()
      .WithMessage("*Mapster data mapping configuration is invalid*WidgetPart*WidgetPartDocument*");
  }

  [Fact]
  public void AddDataMapster_ShouldThrow_WhenADestinationMemberHasNoSource()
  {
    // Act
    Action act = () => new ServiceCollection().AddDataMapster(config =>
    {
      config.NewConfig<Widget, WidgetDocument>();   // Label has no source
      config.NewConfig<WidgetPart, WidgetPartDocument>();
    });

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*Label*");
  }

  [Fact]
  public void AddDataMapster_ShouldThrow_WhenAReversedMemberHasNoSource()
  {
    // Act
    // Configured before TwoWays(), the rename only applies in one direction: Widget.Name has no source.
    Action act = () => new ServiceCollection().AddDataMapster(config =>
    {
      config.NewConfig<Widget, WidgetDocument>()
        .Map(document => document.Label, widget => widget.Name)
        .TwoWays();
      config.NewConfig<WidgetPart, WidgetPartDocument>().TwoWays();
    });

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("*Name*");
  }

  [Fact]
  public void AddDataMapster_ShouldAllowRelaxingTheStrictSettings()
  {
    // Act
    Action act = () => new ServiceCollection().AddDataMapster(config =>
    {
      config.RequireExplicitMapping = false;
      config.RequireDestinationMemberSource = false;
    });

    // Assert
    act.Should().NotThrow();
  }

  [Fact]
  public void AddDataMapster_ShouldApplyTheRegistersOfTheScannedAssemblies()
  {
    // Arrange
    IDataMapper mapper = new ServiceCollection()
      .AddDataMapster(typeof(WidgetMapping).Assembly)
      .BuildServiceProvider()
      .CreateScope().ServiceProvider.GetRequiredService<IDataMapper>();
    Widget widget = new() { Id = Guid.NewGuid(), Name = "name", Part = new WidgetPart { Size = 7 } };

    // Act
    WidgetDocument document = mapper.Map<Widget, WidgetDocument>(widget);

    // Assert
    document.Label.Should().Be(widget.Name);
    document.Part.Size.Should().Be(widget.Part.Size);
  }

  [Fact]
  public void AddDataMapster_ShouldThrow_WhenNoAssemblyIsGiven()
  {
    // Act
    Action act = () => new ServiceCollection().AddDataMapster(Array.Empty<System.Reflection.Assembly>());

    // Assert
    act.Should().Throw<ArgumentException>();
  }

  #endregion

  #region MongoDBMapsterRepository

  [Fact]
  public void Repository_ShouldThrow_WhenTheEntityToDocumentRuleIsMissing()
  {
    // Arrange
    IDataMapper mapper = CreateMapper(config =>
    {
      config.NewConfig<WidgetDocument, Widget>().Map(widget => widget.Name, document => document.Label);
      config.NewConfig<WidgetPartDocument, WidgetPart>();
    });

    // Act
    Action act = () => _ = new MongoDBMapsterRepository<WidgetDocument, Widget, Guid>(Mock.Of<IDataContext>(), mapper);

    // Assert
    act.Should().Throw<InvalidOperationException>()
      .WithMessage("No Mapster mapping from 'Widget' to 'WidgetDocument'*NewConfig<Widget, WidgetDocument>().TwoWays()*");
  }

  [Fact]
  public void Repository_ShouldThrow_WhenTheDocumentToEntityRuleIsMissing()
  {
    // Arrange
    IDataMapper mapper = CreateMapper(config =>
    {
      config.NewConfig<Widget, WidgetDocument>().Map(document => document.Label, widget => widget.Name);
      config.NewConfig<WidgetPart, WidgetPartDocument>();
    });

    // Act
    Action act = () => _ = new MongoDBMapsterRepository<WidgetDocument, Widget, Guid>(Mock.Of<IDataContext>(), mapper);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("No Mapster mapping from 'WidgetDocument' to 'Widget'*");
  }

  [Fact]
  public void Repository_ShouldThrow_WhenAnImplicitMappingIsInvalid()
  {
    // Arrange
    // Implicit mappings are allowed, but Widget.Name still has no source in WidgetDocument.
    IDataMapper mapper = CreateMapper(config => config.RequireExplicitMapping = false);

    // Act
    Action act = () => _ = new MongoDBMapsterRepository<WidgetDocument, Widget, Guid>(Mock.Of<IDataContext>(), mapper);

    // Assert
    act.Should().Throw<InvalidOperationException>().WithMessage("The implicit Mapster mapping*");
  }

  [Fact]
  public async Task Repository_ShouldMapBothWays_WithMapster()
  {
    // Arrange
    Mock<IDataContext> context = new();
    MongoDBMapsterRepository<WidgetDocument, Widget, Guid> repository = new(context.Object, CreateMapper(ConfigureWidget));
    Widget widget = new() { Id = Guid.NewGuid(), Name = "name", Part = new WidgetPart { Size = 7 } };

    WidgetDocument? stored = null;
    context
      .Setup(dataContext => dataContext.InsertAsync<WidgetDocument, Guid>(It.IsAny<WidgetDocument>(), It.IsAny<CancellationToken>()))
      .Callback<WidgetDocument, CancellationToken>((document, _) => stored = document)
      .Returns(Task.CompletedTask);
    context
      .Setup(dataContext => dataContext.FindAsync<WidgetDocument, Guid>(widget.Id, It.IsAny<CancellationToken>()))
      .ReturnsAsync(() => stored);

    // Act
    await repository.InsertAsync(widget);
    Widget? found = await repository.FindAsync(widget.Id);

    // Assert
    stored!.Label.Should().Be(widget.Name);
    stored.Part.Should().NotBeNull().And.NotBeSameAs(widget.Part);
    found.Should().BeEquivalentTo(widget);
  }

  #endregion

  #region Independence

  [Theory]
  [InlineData(typeof(MongoDBRepository<,>))]
  [InlineData(typeof(DataMongoDBServiceCollectionExtensions))]
  public void DirectRepository_ShouldNotDependOnMapster(Type type)
  {
    // Act
    IEnumerable<string?> references = type.Assembly.GetReferencedAssemblies().Select(assembly => assembly.Name);

    // Assert
    references.Should().NotContain(name => name!.StartsWith("Mapster", StringComparison.Ordinal));
  }

  #endregion
}

/// <summary>Found by <c>AddDataMapster(assembly)</c>.</summary>
public sealed class WidgetMapping : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    config.NewConfig<Widget, WidgetDocument>()
      .TwoWays()
      .Map(document => document.Label, widget => widget.Name);
    config.NewConfig<WidgetPart, WidgetPartDocument>().TwoWays();
  }
}
