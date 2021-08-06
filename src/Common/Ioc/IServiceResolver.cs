namespace CodeArchitects.Platform.Common.Ioc
{
  /// <summary>
  /// Interface for resolving a service by name.
  /// </summary>
  /// <typeparam name="TService">The type of the service to resolve.</typeparam>
  public interface IServiceResolver<out TService>
  {
    /// <summary>
    /// Resolves a service by name.
    /// </summary>
    /// <param name="name">The name of the service.</param>
    /// <returns>An instance of <typeparamref name="TService"/> having the specified name.</returns>
    TService Resolve(string name);
  }
}
