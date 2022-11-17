using Xunit.Abstractions;

namespace CodeArchitects.Platform.Data.Fixtures;

public class TestLocalData
{
  private bool _isExecutingTest;

  public ITestOutputHelper? Output { get; private set; }
  public MultitenancyContext MultitenancyContext { get; } = new();
  public SoftDeleteContext SoftDeleteContext { get; } = new();

  public void Initialize(ITestOutputHelper? output)
  {
    if (_isExecutingTest)
      throw new InvalidOperationException("Another test is already executing.");
    _isExecutingTest = true;

    Output = output;
  }

  public void Reset()
  {
    Output = null;
    MultitenancyContext.TenantId = default;
    SoftDeleteContext.ShouldFilter = true;

    _isExecutingTest = false;
  }
}
