using CodeArchitects.Platform.Data.Features.Multitenancy;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class MultitenancyContextWrapper : IMultitenancyContext, IMultitenancyContextBypasser
{
  private readonly IMultitenancyContext _context;
  private bool _isBypassed;

  public MultitenancyContextWrapper(IMultitenancyContext context)
  {
    _context = context;
  }

  public bool ShouldFilter => !_isBypassed && _context.ShouldFilter;

  public object TenantId => _context.TenantId;

  public IDisposable BypassMultitenancy()
  {
    return new BypassScope(this);
  }

  private sealed class BypassScope : IDisposable
  {
    private readonly MultitenancyContextWrapper _context;

    public BypassScope(MultitenancyContextWrapper context)
    {
      _context = context;
      _context._isBypassed = true;
    }

    public void Dispose()
    {
      _context._isBypassed = false;
    }
  }
}
