using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore;

public class Seeder : ISeeder
{
  private readonly DbContext _context;

  public Seeder(DbContext context)
  {
    _context = context;
  }

  public void Seed<T>(IEnumerable<T> entities) where T : class
  {
    if (!_context.Set<T>().Any())
    {
      _context.AddRange(entities);
    }
  }
}
