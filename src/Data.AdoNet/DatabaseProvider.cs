namespace CodeArchitects.Platform.Data.AdoNet;

public abstract class DatabaseProvider
{
  private protected DatabaseProvider() { }

  private protected abstract Type DataContextType { get; }

  private protected abstract Type SyntaxProviderType { get; }
}
