using MongoDB.Driver;

namespace CodeArchitects.Platform.Data.MongoDB;

internal class Seeder : ISeeder
{
  private readonly IDataContext _context;

  public Seeder(IDataContext context)
  {
    _context = context;
  }

  public void Seed<TEntity>(IEnumerable<TEntity> entities)
    where TEntity : class
  {
    IMongoCollection<TEntity> collection = _context.GetCollection<TEntity>();
    if (collection.EstimatedDocumentCount() == 0)
    {
      collection.InsertMany(entities);
    }
  }
}
