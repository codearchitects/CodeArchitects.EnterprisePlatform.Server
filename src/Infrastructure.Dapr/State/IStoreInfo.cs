namespace CodeArchitects.Platform.Infrastructure.Dapr.State;

/// <summary>
/// Provides information about state stores.
/// </summary>
internal interface IStoreInfo
{
  /// <summary>
  /// Checks whether a state store is known.
  /// </summary>
  /// <param name="storeName">The name of the state store.</param>
  /// <returns><c>true</c> if the state store is known, <c>false</c> otherwise.</returns>
  bool IsStoreKnown(string storeName);

  /// <summary>
  /// Gets the default state store.
  /// </summary>
  /// <returns>The name of the default state store.</returns>
  string? GetDefaultStore();
}
