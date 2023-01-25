using CodeArchitects.Platform.Data;
using EFCoreSample.Domain.Model;

namespace EFCoreSample.Domain.Repositories;

public interface IProductRepository : IRepository<Product, Guid>
{
  Task<IEnumerable<Product>> GetProductsWithPriceLessThan(decimal price, CancellationToken cancellationToken = default);
  Task<IEnumerable<Product>> GetPublicProducts(CancellationToken cancellationToken = default);
}
