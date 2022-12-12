using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public interface IEFCoreContext : IDataContext
{
  DbContext DbContext { get; }
}