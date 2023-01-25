namespace CodeArchitects.Platform.Data;

/// <summary>
/// Manages the creation and disposal of unit of work instances.
/// </summary>
public interface IUnitOfWorkManager
{
  /// <summary>
  /// Begins a new unit of work.
  /// </summary>
  /// <remarks>
  /// This method should be used in combination with the <c>await using</c> keywords.
  /// All changes made to the entities that support the unit of work will be persisted when the <see cref="IUnitOfWork.SaveAsync(CancellationToken)"/> method completes.
  /// </remarks>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A new <see cref="IUnitOfWork"/> instance.</returns>
  IUnitOfWork Begin(CancellationToken cancellationToken = default);

  /// <summary>
  /// Begins a new unit of work.
  /// </summary>
  /// <remarks>
  /// This method should be used in combination with the <c>await using</c> keywords.
  /// All changes made to the entities that support the unit of work will be persisted when the <see cref="IUnitOfWork.SaveAsync(CancellationToken)"/> method completes.
  /// </remarks>
  /// <param name="autoSave">Indicates whether changes made to entities in the unit of work should be automatically saved to the data store when the unit of work is disposed.</param>
  /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the operation.</param>
  /// <returns>A new <see cref="IUnitOfWork"/> instance.</returns>
  IUnitOfWork Begin(bool autoSave, CancellationToken cancellationToken = default);
}
