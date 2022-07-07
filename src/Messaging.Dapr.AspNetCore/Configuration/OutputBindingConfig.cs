using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Messaging.Dapr.AspNetCore.Configuration;

/// <summary>
/// Configuration of an output action.
/// </summary>
public class OutputBindingConfig
{
  /// <summary>
  /// The fully qualified name of the binding metadata.
  /// </summary>
  [Required]
  public string Name { get; set; } = default!;

  /// <summary>
  /// The metadata object.
  /// </summary>
  public Dictionary<string, object> Metadata { get; set; } = new();
}
