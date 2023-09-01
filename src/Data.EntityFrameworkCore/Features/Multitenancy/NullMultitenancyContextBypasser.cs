namespace CodeArchitects.Platform.Data.EntityFrameworkCore.Features.Multitenancy;

internal class NullMultitenancyContextBypasser : IMultitenancyContextBypasser
{
  public IDisposable BypassMultitenancy() => BypassScope.Instance;

  private sealed class BypassScope : IDisposable
  {
    public static readonly BypassScope Instance = new BypassScope();

    private BypassScope() { }

    public void Dispose()
    {
      // Nothing to do
    }
  }
}
