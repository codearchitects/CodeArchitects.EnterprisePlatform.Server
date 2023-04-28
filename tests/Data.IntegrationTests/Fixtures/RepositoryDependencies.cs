using CodeArchitects.Platform.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CodeArchitects.Platform.Data.Fixtures;

public record RepositoryDependencies(
  RepositoryImplementation Implementation,
  DbProvider Provider)
{
  public Type ConcurrencyExceptionType => Implementation switch
  {
    RepositoryImplementation.AdoNet => typeof(DBConcurrencyException),
    RepositoryImplementation.EFCore => typeof(DbUpdateConcurrencyException),
    _                               => throw Errors.Unreachable
  };
}
