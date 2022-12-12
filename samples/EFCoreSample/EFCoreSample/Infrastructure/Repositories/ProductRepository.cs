using CodeArchitects.Platform.Data.EntityFrameworkCore;
using CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;
using EFCoreSample.Domain.Model;
using EFCoreSample.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EFCoreSample.Infrastructure.Repositories;

public class ProductRepository : EFCoreRepository<Product, Guid>, IProductRepository
{
  public ProductRepository(IEFCoreContext context)
    : base(context)
  {
  }

  public async Task<IEnumerable<Product>> GetProductsWithPriceLessThan(decimal price, CancellationToken cancellationToken = default)
  {
    return await Entities
      .Where(product => product.Price < price)
      .ToListAsync(cancellationToken);
  }

  public async Task<IEnumerable<Product>> GetPublicProducts(CancellationToken cancellationToken = default)
  {
    return await Entities
      .AsNoMultitenancy()
      .Where(product => product.IsPublic)
      .ToListAsync(cancellationToken);
  }
}
