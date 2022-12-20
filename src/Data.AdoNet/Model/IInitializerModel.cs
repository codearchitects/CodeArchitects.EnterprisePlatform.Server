using CodeArchitects.Platform.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
/// <summary>
/// Represents a model for initializing an entity instance.
/// </summary>
public interface IInitializerModel
{
  /// <summary>
  /// The constructor used to create the instance.
  /// </summary>
  ConstructorInfo Constructor { get; }

  /// <summary>
  /// The properties passed as arguments to the constructor.
  /// </summary>
  IReadOnlyCollection<IAccessibleColumnModel> ConstructorProperties { get; }

  /// <summary>
  /// The properties that need to be initialized after the instance is created.
  /// </summary>
  IReadOnlyCollection<IAccessibleColumnModel> InitializerProperties { get; }
}
