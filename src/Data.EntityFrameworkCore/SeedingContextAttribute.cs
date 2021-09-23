using Microsoft.EntityFrameworkCore;
using System;

namespace CodeArchitects.Platform.Data.EntityFrameworkCore
{
  [AttributeUsage(AttributeTargets.Class)]
  public class SeedingContextAttribute : Attribute
  {
    public SeedingContextAttribute(Type contextType)
    {
      if (contextType is null) throw new ArgumentNullException(nameof(contextType));
      if (contextType.IsAssignableTo(typeof(DbContext))) throw new ArgumentException($"{contextType.FullName} does not extends {typeof(DbContext).FullName}.", nameof(contextType));
      
      ContextType = contextType;
    }

    public Type ContextType { get; }
  }
}
