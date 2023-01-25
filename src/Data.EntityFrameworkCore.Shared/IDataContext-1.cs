using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// Represents a <see cref="Data.IDataContext"/> that is based on Entity Framework Core and uses a specific <see cref="Microsoft.EntityFrameworkCore.DbContext"/> type.
/// </summary>
public interface IDataContext<TDbContext> : IDataContext
  where TDbContext : DbContext
{
  /// <summary>
  /// The application <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
  /// </summary>
  new TDbContext DbContext { get; }
}
