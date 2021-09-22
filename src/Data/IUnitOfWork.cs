using System.Threading;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Data
{
  public interface IUnitOfWork
  {
    void Save();
    Task SaveAsync(CancellationToken cancellationToken = default);
  }
}
