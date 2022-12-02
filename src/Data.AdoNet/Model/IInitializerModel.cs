using CodeArchitects.Platform.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IInitializerModel
{
  ConstructorInfo Constructor { get; }
  
  IReadOnlyList<IAccessiblePropertyModel> ConstructorProperties { get; }
  
  IReadOnlyList<IAccessiblePropertyModel> InitializerProperties { get; }
}