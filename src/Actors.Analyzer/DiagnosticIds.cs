namespace CodeArchitects.Platform.Actors.Analyzer;

internal class DiagnosticIds
{
  // Class errors (000-299)
  public const string CAEPACTR000 = nameof(CAEPACTR000); // Duplicate actor attribute
  public const string CAEPACTR001 = nameof(CAEPACTR001); // Generic actors are not supported
  public const string CAEPACTR002 = nameof(CAEPACTR002); // Missing actor interface
  public const string CAEPACTR003 = nameof(CAEPACTR003); // Ambiguous actor interface
  public const string CAEPACTR004 = nameof(CAEPACTR004); // Interface not implemented
  public const string CAEPACTR005 = nameof(CAEPACTR005); // Interface type is not an interface
  public const string CAEPACTR006 = nameof(CAEPACTR006); // Properties are not supported
  public const string CAEPACTR007 = nameof(CAEPACTR007); // Events are not supported
  public const string CAEPACTR008 = nameof(CAEPACTR008); // Actor cannot be virtual
  public const string CAEPACTR009 = nameof(CAEPACTR009); // Duplicate actor implementation attribute
  public const string CAEPACTR010 = nameof(CAEPACTR010); // Multiple default implementations
  public const string CAEPACTR011 = nameof(CAEPACTR011); // Actor not inherited
  public const string CAEPACTR012 = nameof(CAEPACTR012); // Abstract implementation

  // State/id errors (300-399)
  public const string CAEPACTR300 = nameof(CAEPACTR300); // Invalid state type
  public const string CAEPACTR301 = nameof(CAEPACTR301); // Invalid default value
  public const string CAEPACTR302 = nameof(CAEPACTR302); // State must be defined in base actor
  public const string CAEPACTR303 = nameof(CAEPACTR303); // Ambiguous actor id source
  public const string CAEPACTR304 = nameof(CAEPACTR304); // Invalid id type
  public const string CAEPACTR305 = nameof(CAEPACTR305); // Multiple id source interfaces
  public const string CAEPACTR306 = nameof(CAEPACTR306); // Invalid id source
  public const string CAEPACTR307 = nameof(CAEPACTR307); // Duplicate ActorIdType attribute

  // Method or constructor errors (400-599)
  public const string CAEPACTR400 = nameof(CAEPACTR400); // State component name mismatch
  public const string CAEPACTR401 = nameof(CAEPACTR401); // Ambiguous actor constructor
  public const string CAEPACTR402 = nameof(CAEPACTR402); // Wrong generic actor context
  public const string CAEPACTR403 = nameof(CAEPACTR403); // Generic methods are not supported
  public const string CAEPACTR404 = nameof(CAEPACTR404); // Invalid method return type
  public const string CAEPACTR405 = nameof(CAEPACTR405); // CancellationToken must be last parameter

  // Factory errors (600-699)
  public const string CAEPACTR600 = nameof(CAEPACTR600); // Duplicate actor factory attribute
  public const string CAEPACTR601 = nameof(CAEPACTR601); // Generic actor factories are not supported
  public const string CAEPACTR602 = nameof(CAEPACTR602); // Missing actor factory type
  public const string CAEPACTR603 = nameof(CAEPACTR603); // Invalid actor factory type
  public const string CAEPACTR604 = nameof(CAEPACTR604); // Ambiguous actor factory type

  // Other errors (700-999)
  public const string CAEPACTR700 = nameof(CAEPACTR700); // Invalid actor message
}
