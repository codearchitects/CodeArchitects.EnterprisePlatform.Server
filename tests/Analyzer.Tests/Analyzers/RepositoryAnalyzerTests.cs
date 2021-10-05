using CodeArchitects.Platform.Analyzer.Analyzers;
using CodeArchitects.Platform.Data;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace CodeArchitects.Platform.Analyzer.Tests.Analyzers
{
  public class RepositoryAnalyzerTests : AnalyzerTest
  {
    protected override Type AnalyzerType => typeof(RepositoryAnalyzer);

    protected override IEnumerable<Type> ReferencedAssemblyMarkers => new Type[]
    {
      typeof(IRepository<,>),
      typeof(IQueryable<>)
    };

    protected override IEnumerable<Assembly> ReferencedAsseblies => new Assembly[]
    {
      Assembly.Load("System.Runtime")
    };

    [Fact]
    public async Task EmptyCode_ShouldNotTriggerCAESP002()
    {
      // Arrange
      const string code = @"
using CodeArchitects.Platform.Data;

namespace Test
{
  public static class Program
  {
    public static void Main(string[] args)
    {
    }
  }
}
";

      // Act
      ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

      // Assert
      diagnostics.Should().NotContain(x => x.Id == DiagnosticIds.CAESP002);
    }

    [Fact]
    public async Task InterfaceThatDoesNotExposeIQueryable_ShouldNotTriggerCAESP002()
    {
      // Arrange
      const string code = @"
using CodeArchitects.Platform.Data;

namespace Test
{
  public class Entity : IEntity<int>
  {
    public int Id { get; }
    object IEntity.Id => Id;
  }

  public interface IEntityRepository : IRepository<Entity, int>
  {
    Entity MyQuery();
  }
}
";

      // Act
      ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

      // Assert
      diagnostics.Should().NotContain(x => x.Id == DiagnosticIds.CAESP002);
    }

    [Fact]
    public async Task InterfaceThatExposesIQueryableAsMethod_ShouldTriggerCAESP002()
    {
      // Arrange
      const string code = @"
using CodeArchitects.Platform.Data;
using System.Linq;

namespace Test
{
  public class Entity : IEntity<int>
  {
    public int Id { get; }
    object IEntity.Id => Id;
  }

  public interface IEntityRepository : IRepository<Entity, int>
  {
    Entity MyQuery();
    IQueryable<Entity> Query();
  }
}
";

      // Act
      ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

      // Assert
      diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP002);
    }

    [Fact]
    public async Task InterfaceThatExposesIQueryableAsProperty_ShouldTriggerCAESP002()
    {
      // Arrange
      const string code = @"
using CodeArchitects.Platform.Data;
using System.Linq;

namespace Test
{
  public class Entity : IEntity<int>
  {
    public int Id { get; }
    object IEntity.Id => Id;
  }

  public interface IEntityRepository : IRepository<Entity, int>
  {
    Entity MyQuery();
    IQueryable<Entity> Query { get; }
  }
}
";

      // Act
      ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

      // Assert
      diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP002);
    }

    [Fact]
    public async Task InterfaceThatExposesIQueryableNTimes_ShouldTriggerCAESP002NTimes()
    {
      // Arrange
      const string code = @"
using CodeArchitects.Platform.Data;
using System.Linq;

namespace Test
{
  public class Entity : IEntity<int>
  {
    public int Id { get; }
    object IEntity.Id => Id;
  }

  public interface IEntityRepository : IRepository<Entity, int>
  {
    Entity MyQuery();
    IQueryable<Entity> Query1();
    IQueryable<Entity> Query2 { get; }
    IQueryable<Entity> Query3();
  }
}
";

      // Act
      ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

      // Assert
      diagnostics.Where(x => x.Id == DiagnosticIds.CAESP002).Should().HaveCount(3);
    }

    [Fact]
    public async Task ClassThatExposesIQueryable_ShouldNotTriggerCAESP002()
    {
      // Arrange
      const string code = @"
using CodeArchitects.Platform.Data;
using System.Linq;

namespace Test
{
  public class Entity : IEntity<int>
  {
    public int Id { get; }
    object IEntity.Id => Id;
  }

  public interface IEntityRepository : IRepository<Entity, int>
  {
    Entity MyQuery();
    IQueryable<Entity> Query();
  }

  public class EntityRepository : Repository<Entity, int> : IEntityRepository
  {
    public EntityRepository(DbContext context) : base(context) { }

    public Entity MyQuery => default;

    public IQueryable<Entity> Query() => default;
  }
}
";

      // Act
      ImmutableArray<Diagnostic> diagnostics = await GetDiagnosticsAsync(code);

      // Assert
      diagnostics.Should().ContainSingle(x => x.Id == DiagnosticIds.CAESP002);
    }
  }
}
