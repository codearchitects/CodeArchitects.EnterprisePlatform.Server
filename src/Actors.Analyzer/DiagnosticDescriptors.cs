#pragma warning disable RS2008 // Enable analyzer release tracking

using Microsoft.CodeAnalysis;

namespace CodeArchitects.Platform.Actors.Analyzer;

internal static class DiagnosticDescriptors
{
  // Class errors (000-299)

  public static readonly DiagnosticDescriptor CAEPACTR000 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR000,
    "Duplicate actor attribute",
    "Duplicate 'Actor' attribute",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR001 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR001,
    "Generic actors are not supported",
    "Actor '{0}' is generic, which is not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR002 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR002,
    "Missing actor interface",
    "An actor must implement an interface which exposes its public methods, but '{0}' does not implement any interface",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR003 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR003,
    "Ambiguous actor interface",
    "Could not infer the actor interface type because '{0}' implements more than one interface",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR004 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR004,
    "Interface not implemented",
    "'{0}' does not implement the specified interface type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR005 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR005,
    "Interface type is not an interface",
    "'{0}' was specified as the interface type of the actor, but it is not an interface",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR006 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR006,
    "Properties are not supported",
    "Interface '{0}' defines properties, which are not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR007 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR007,
    "Events are not supported",
    "Interface '{0}' defines events, which are not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR008 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR008,
    "Actor cannot be virtual",
    "'{0}' cannot be a virtual actor because a default value for its state cannot be computed. For simple state types, either configure the its default value or provide a default parameter value. For complex state types, provide a parameterless constructor for the class.",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR009 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR009,
    "Duplicate ActorImplementation attribute",
    "Duplicate 'ActorImplementation' attribute",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR010 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR010,
    "Multiple default implementations",
    "Multiple default implementations found for actor '{0}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR011 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR011,
    "Invalid implementation",
    "Type '{0}' does not inherit from the actor type it specifies",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR012 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR012,
    "Abstract implementation",
    "Type '{0}' cannot be an actor implementation since it is abstract",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);


  // State/id errors (300-399)

  public static readonly DiagnosticDescriptor CAEPACTR300 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR300,
    "Invalid state type",
    "Type '{0}' cannot be used as an actor state",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR301 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR301,
    "Invalid default value",
    "The provided default value for state component '{0}' is of the wrong type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR302 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR302,
    "State must be defined in base actor",
    "Each component of the actor's state must be defined in the actor's base class. Implementation '{0}' defines state members.",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR303 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR303,
    "Ambiguous actor id source",
    "Multiple actor id sources were set. Only one state component or one state component property can be configured to be the actor id.",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR304 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR304,
    "Invalid id type",
    "Type '{0}' cannot be used as id type because it does not define a public static method Parse(string) or Parse(string, IFormatProvider) returning the id type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR305 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR305,
    "Multiple id source interfaces",
    "Type '{0}' implements IActorIdSource multiple times",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR306 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR306,
    "Invalid id source",
    "An id source provides an id of type '{0}', but the actor id type is '{1}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR307 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR307,
    "Duplicate ActorIdType attribute",
    "Duplicate 'ActorIdType' attribute",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);


  // Method or constructor errors (400-599)

  public static readonly DiagnosticDescriptor CAEPACTR400 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR400,
    "State component name mismatch",
    "Could not find a one-to-one correspondence between state members and state constructor parameters",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR401 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR401,
    "Ambiguous actor constructor",
    "Could not infer the actor constructor",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR402 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR402,
    "Wrong generic actor context",
    "The actor context parameter '{1}' must be of type 'IActorContext<{0}>'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR403 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR403,
    "Generic methods are not supported",
    "Method '{0}' is generic, which is not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR404 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR404,
    "Invalid method return type",
    "Method '{0}' has an invalid return type '{1}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR405 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR405,
    "CancellationToken must be last parameter",
    "Method '{0}' has a CancellationToken parameter which is not its last parameter",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);


  // Factory errors (600-699)

  public static readonly DiagnosticDescriptor CAEPACTR600 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR600,
    "Duplicate ActorFactory attribute",
    "Duplicate 'ActorFactory' attribute on type '{0}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR601 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR601,
    "Generic actor factories are not supported",
    "Actor factory '{0}' is generic, which is not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR602 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR602,
    "Missing actor factory type",
    "Could not find a suitable factory interface for '{0}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR603 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR603,
    "Invalid actor factory type",
    "Actor factory '{0}' is not an interface or has the wrong method signatures",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR604 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR604,
    "Ambiguous actor factory type",
    "Multiple actor factories were specified for actor '{0}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);


  // Other errors (700-999)

  public static readonly DiagnosticDescriptor CAEPACTR700 = new DiagnosticDescriptor(
    DiagnosticIds.CAEPACTR700,
    "Invalid actor message",
    "Message type '{0}' does not implement the IActorMessage interface or it has the wrong actor id type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);
}
