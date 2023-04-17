namespace CodeArchitects.Platform.Actors.Analyzer;

internal static class DiagnosticIds
{
  // Class errors (000-299)
  public const string CAEPACTR000 = nameof(CAEPACTR000); // Generic actors are not supported
  public const string CAEPACTR001 = nameof(CAEPACTR001); // Missing actor interface
  public const string CAEPACTR002 = nameof(CAEPACTR002); // Ambiguous actor interface
  public const string CAEPACTR003 = nameof(CAEPACTR003); // Interface not implemented
  public const string CAEPACTR004 = nameof(CAEPACTR004); // Invalid interface type
  public const string CAEPACTR005 = nameof(CAEPACTR005); // Properties are not supported
  public const string CAEPACTR006 = nameof(CAEPACTR006); // Events are not supported
  public const string CAEPACTR007 = nameof(CAEPACTR007); // Actor cannot be virtual
  public const string CAEPACTR008 = nameof(CAEPACTR008); // Multiple default implementations
  public const string CAEPACTR009 = nameof(CAEPACTR009); // Actor not inherited
  public const string CAEPACTR010 = nameof(CAEPACTR010); // Abstract implementation

  // State/id errors (300-399)
  public const string CAEPACTR300 = nameof(CAEPACTR300); // Invalid state type
  public const string CAEPACTR301 = nameof(CAEPACTR301); // Invalid default value
  public const string CAEPACTR302 = nameof(CAEPACTR302); // State must be defined in base actor
  public const string CAEPACTR303 = nameof(CAEPACTR303); // Multiple id members
  public const string CAEPACTR304 = nameof(CAEPACTR304); // Invalid id type
  public const string CAEPACTR305 = nameof(CAEPACTR305); // Invalid id member

  // Method or constructor errors (400-599)
  public const string CAEPACTR400 = nameof(CAEPACTR400); // State component name mismatch
  public const string CAEPACTR401 = nameof(CAEPACTR401); // Ambiguous actor constructor
  public const string CAEPACTR402 = nameof(CAEPACTR402); // Wrong generic actor context
  public const string CAEPACTR403 = nameof(CAEPACTR403); // Generic methods are not supported
  public const string CAEPACTR404 = nameof(CAEPACTR404); // Invalid method return type
  public const string CAEPACTR405 = nameof(CAEPACTR405); // CancellationToken must be last parameter

  // Factory errors (600-699)
  public const string CAEPACTR600 = nameof(CAEPACTR600); // Generic actor factories are not supported
  public const string CAEPACTR601 = nameof(CAEPACTR601); // Missing actor factory type
  public const string CAEPACTR602 = nameof(CAEPACTR602); // Invalid actor factory type
  public const string CAEPACTR603 = nameof(CAEPACTR603); // Ambiguous actor factory type

  // Other errors (700-999)
  public const string CAEPACTR700 = nameof(CAEPACTR700); // Duplicate attribute
  public const string CAEPACTR701 = nameof(CAEPACTR701); // Target is not an actor
  public const string CAEPACTR702 = nameof(CAEPACTR702); // Invalid actor message
}
