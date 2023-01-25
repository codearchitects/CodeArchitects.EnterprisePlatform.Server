using CodeArchitects.Platform.Data;
using EFCoreSample.Domain.Model;

namespace EFCoreSample.Domain.Repositories;

public interface ICartRepository : IRepository<Cart, Guid>
{
}
