using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

/// <summary>
/// Represents a <see cref="Data.IDataContext"/> that is based on Entity Framework Core.
/// </summary>
public interface IDataContext : Data.IDataContext
{
  /// <summary>
  /// The application <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
  /// </summary>
  DbContext DbContext { get; }
}