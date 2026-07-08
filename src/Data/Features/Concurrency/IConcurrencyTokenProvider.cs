namespace CodeArchitects.Platform.Data.Features.Concurrency;

/// <summary>
/// Provides tokens for optimistic concurrency checks.
/// </summary>
public interface IConcurrencyTokenProvider
{
  /// <summary>
  /// Creates a pseudo-random instance of the specified type that will be used as a concurrency token.
  /// </summary>
  /// <param name="tokenType">The concurrency token type.</param>
  /// <param name="previousToken">The previous value of the concurrency token.</param>
  /// <returns>The new concurrency token.</returns>
  object CreateToken(Type tokenType, object? previousToken);
}

