using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public interface IDataContext : Data.IDataContext
{
  DbContext DbContext { get; }
}