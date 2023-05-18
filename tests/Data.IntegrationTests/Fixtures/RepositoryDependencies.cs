using CodeArchitects.Platform.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodeArchitects.Platform.Data.Fixtures;

public record RepositoryDependencies(
  DataImplementation Implementation,
  DbProvider Provider,
  bool Async)
{
  public Type ConcurrencyExceptionType => Implementation switch
  {
    DataImplementation.AdoNet => typeof(DBConcurrencyException),
    DataImplementation.EFCore => typeof(DbUpdateConcurrencyException),
    _                         => throw Errors.Unreachable
  };
}
