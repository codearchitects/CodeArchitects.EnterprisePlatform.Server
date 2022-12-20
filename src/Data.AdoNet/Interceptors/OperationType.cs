namespace CodeArchitects.Platform.Data.AdoNet.Interceptors;

/// <summary>
/// Represents an operation performed by the data context.
/// </summary>
public enum OperationType
{
  /// <summary>
  /// The Find operation.
  /// </summary>
  Find,

  /// <summary>
  /// The Insert operation.
  /// </summary>
  Insert,

  /// <summary>
  /// The Update operation.
  /// </summary>
  Update,

  /// <summary>
  /// The Remove operation.
  /// </summary>
  Remove
}
