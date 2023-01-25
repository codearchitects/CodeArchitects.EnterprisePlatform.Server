using Microsoft.EntityFrameworkCore.Query;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Navigation;

internal delegate IIncludableQueryable<TParent, T> ReferenceInclude<TParent, T>(IQueryable<TParent> queryable)
  where TParent : class
  where T : class;

internal delegate IIncludableQueryable<TParent, IEnumerable<T>> CollectionInclude<TParent, T>(IQueryable<TParent> queryable)
  where TParent : class
  where T : class;
