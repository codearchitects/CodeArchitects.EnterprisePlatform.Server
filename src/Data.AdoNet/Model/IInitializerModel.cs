using CodeArchitects.Platform.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IInitializerModel
{
  ConstructorInfo Constructor { get; }
  
  IReadOnlyCollection<IAccessibleColumnModel> ConstructorProperties { get; }
  
  IReadOnlyCollection<IAccessibleColumnModel> InitializerProperties { get; }
}