using CodeArchitects.Platform.Data.EntityFrameworkCore;
using EFCoreSample.Domain.Model;
using EFCoreSample.Domain.Repositories;

namespace EFCoreSample.Infrastructure.Repositories;

public class CartRepository : EFCoreRepository<Cart, Guid>, ICartRepository
{
  public CartRepository(IDataContext context)
    : base(context)
  {
  }
}
