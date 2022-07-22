using System.ComponentModel.DataAnnotations;

namespace CodeArchitects.Platform.Messaging.AspNetCore.Configuration;

/// <summary>
/// Input and output bindings for a message handler.
/// </summary>
public class HandlerBindingsConfig
{
  /// <summary>
  /// The name of the bus to subscribe the handler to.
  /// </summary>
  public string? Bus { get; set; }

  /// <summary>
  /// The name of the topic to subscribe the handler to.
  /// </summary>
  public string? Topic { get; set; }

  /// <summary>
  /// The fully qualified name of the message type.
  /// </summary>
  [Required]
  public string MessageType { get; set; } = default!;

  /// <summary>
  /// The fully qualified name of the result type.
  /// </summary>
  public string? ResultType { get; set; }

  /// <summary>
  /// The fully qualified names of the result types, if a union type is used.
  /// </summary>
  public List<string> ResultTypes { get; set; } = new();

  /// <summary>
  /// The list of the output actions the handler will be bound to.
  /// </summary>
  public List<OutputBindingConfig> Output { get; set; } = new();
}
