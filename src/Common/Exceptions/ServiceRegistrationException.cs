using System;
using System.Collections.Generic;

namespace CodeArchitects.Platform.Common.Exceptions
{
  /// <summary>
  /// The exception that is thrown when the service registration configuration is invalid.
  /// </summary>
  public class ServiceRegistrationException : Exception
  {
    /// <summary>
    /// Creates a new <see cref="ServiceRegistrationException"/> instance with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="implementationTypes">The invalid implementation types.</param>
    public ServiceRegistrationException(string message, IEnumerable<Type> implementationTypes)
      : base(message)
    {
      ImplementationTypes = implementationTypes;
    }

    /// <summary>
    /// Creates a new <see cref="ServiceRegistrationException"/>.
    /// </summary>
    /// <param name="implementationTypes">The invalid implementation types.</param>
    public ServiceRegistrationException(IEnumerable<Type> implementationTypes)
    {
      ImplementationTypes = implementationTypes;
    }

    /// <summary>
    /// The set of invalid implementation type.
    /// </summary>
    public IEnumerable<Type> ImplementationTypes { get; }
  }
}
