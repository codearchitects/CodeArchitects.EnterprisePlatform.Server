using Microsoft.EntityFrameworkCore;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Utils
{
  public static class DbContextExtensions
  {
    public static int SaveChangesAndClearTracking(this DbContext context)
    {
      int result = context.SaveChanges();
      context.ChangeTracker.Clear();
      return result;
    }
  }
}
