namespace CodeArchitects.Platform.Data.Features.Concurrency;

/// <summary>
/// Creates a new concurrency token given its previous value.
/// </summary>
/// <param name="previousToken">The previous value of the concurrency token.</param>
/// <returns>The new concurrency token.</returns>
public delegate object ConcurrencyTokenFactory(object? previousToken);
