#pragma warning disable RS2008 // Enable analyzer release tracking

using Microsoft.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Actors.Analyzer;

internal static class DiagnosticDescriptors
{
  static DiagnosticDescriptors()
  {
    All = typeof(DiagnosticDescriptors)
      .GetFields(BindingFlags.Static | BindingFlags.Public)
      .Select(field => field.GetValue(null))
      .OfType<DiagnosticDescriptor>()
      .ToArray();
  }

  public static readonly DiagnosticDescriptor[] All;

  // Class errors (000-299)

  public static readonly DiagnosticDescriptor CAEPACTR000 = new(
    DiagnosticIds.CAEPACTR000,
    "Generic actors are not supported",
    "Actor '{0}' is generic, which is not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR001 = new(
    DiagnosticIds.CAEPACTR001,
    "Missing actor interface",
    "An actor must implement an interface which exposes its public methods, but '{0}' does not implement any suitable interface",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR002 = new(
    DiagnosticIds.CAEPACTR002,
    "Ambiguous actor interface",
    "Could not infer the actor interface type because '{0}' implements more than one interface",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR003 = new(
    DiagnosticIds.CAEPACTR003,
    "Interface not implemented",
    "Type '{0}' does not implement the specified interface type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR004 = new(
    DiagnosticIds.CAEPACTR004,
    "Invalid interface type",
    "'{0}' was specified as the interface type of the actor, but it is either not an interface or an invalid one",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR005 = new(
    DiagnosticIds.CAEPACTR005,
    "Properties are not supported",
    "Interface '{0}' defines properties, which are not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR006 = new(
    DiagnosticIds.CAEPACTR006,
    "Events are not supported",
    "Interface '{0}' defines events, which are not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR007 = new(
    DiagnosticIds.CAEPACTR007,
    "Actor cannot be virtual",
    "'{0}' cannot be a virtual actor because a default value for its state cannot be computed. For simple state types, either configure the its default value or provide a default parameter value. For complex state types, provide a parameterless constructor for the class.",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR008 = new(
    DiagnosticIds.CAEPACTR008,
    "Multiple default implementations",
    "Multiple default implementations found for actor '{0}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR009 = new(
    DiagnosticIds.CAEPACTR009,
    "Actor not inherited",
    "Type '{0}' does not inherit from the specified actor type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR010 = new(
    DiagnosticIds.CAEPACTR010,
    "Abstract implementation",
    "Type '{0}' cannot be an actor implementation since it is abstract",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);


  // State/id errors (300-399)

  public static readonly DiagnosticDescriptor CAEPACTR300 = new(
    DiagnosticIds.CAEPACTR300,
    "Invalid state type",
    "Type '{0}' cannot be used as an actor state",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR301 = new(
    DiagnosticIds.CAEPACTR301,
    "Invalid default value",
    "The provided default value for state component '{0}' is of the wrong type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR302 = new(
    DiagnosticIds.CAEPACTR302,
    "State must be defined in base actor",
    "Each component of the actor's state must be defined in the actor's base class. Implementation '{0}' defines state members.",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR303 = new(
    DiagnosticIds.CAEPACTR303,
    "Multiple id members",
    "Multiple actor id members were specified",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR304 = new(
    DiagnosticIds.CAEPACTR304,
    "Invalid id type",
    "Type '{0}' cannot be used as id type because it does not define a public static method Parse(string) or Parse(string, IFormatProvider) returning the id type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR305 = new(
    DiagnosticIds.CAEPACTR305,
    "Invalid id member",
    "An actor member specified an id of type '{0}', but the declared actor id type is '{1}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR306 = new(
    DiagnosticIds.CAEPACTR306,
    "Null default value",
    "A null literal default value was specified for non-nullable state member '{0}'",
    "Actors",
    DiagnosticSeverity.Warning,
    isEnabledByDefault: true);


  // Method or constructor errors (400-599)

  public static readonly DiagnosticDescriptor CAEPACTR400 = new(
    DiagnosticIds.CAEPACTR400,
    "State component name mismatch",
    "Could not find a one-to-one correspondence between state members and state constructor parameters",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR401 = new(
    DiagnosticIds.CAEPACTR401,
    "Ambiguous actor constructor",
    "Could not infer the actor constructor",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR402 = new(
    DiagnosticIds.CAEPACTR402,
    "Wrong generic actor context",
    "The actor context parameter '{0}' must be of type 'IActorContext<{1}>'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR403 = new(
    DiagnosticIds.CAEPACTR403,
    "Generic methods are not supported",
    "Method '{0}' is generic, which is not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR404 = new(
    DiagnosticIds.CAEPACTR404,
    "Invalid method return type",
    "Method '{0}' has an invalid return type '{1}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR405 = new(
    DiagnosticIds.CAEPACTR405,
    "CancellationToken must be last parameter",
    "Method '{0}' has a CancellationToken parameter which is not its last parameter",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);


  // Factory errors (600-699)

  public static readonly DiagnosticDescriptor CAEPACTR600 = new(
    DiagnosticIds.CAEPACTR600,
    "Generic actor factories are not supported",
    "Actor factory '{0}' is generic, which is not supported",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR601 = new(
    DiagnosticIds.CAEPACTR601,
    "Missing actor factory type",
    "Could not find a suitable factory interface for '{0}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR602 = new(
    DiagnosticIds.CAEPACTR602,
    "Invalid actor factory type",
    "Actor factory '{0}' is not an interface or has the wrong method signatures",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR603 = new(
    DiagnosticIds.CAEPACTR603,
    "Ambiguous actor factory type",
    "Multiple actor factories were specified for actor '{0}'",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);


  // Other errors (700-999)

  public static readonly DiagnosticDescriptor CAEPACTR700 = new(
    DiagnosticIds.CAEPACTR700,
    "Duplicate attribute",
    "Duplicate '{0}' attribute",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR701 = new(
    DiagnosticIds.CAEPACTR701,
    "Target is not an actor",
    "Type '{0}' is not an actor",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);

  public static readonly DiagnosticDescriptor CAEPACTR702 = new(
    DiagnosticIds.CAEPACTR702,
    "Invalid actor message",
    "Message type '{0}' does not implement the IActorMessage interface or it has the wrong actor id type",
    "Actors",
    DiagnosticSeverity.Error,
    isEnabledByDefault: true);
}
