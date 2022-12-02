using CodeArchitects.Platform.CodeAnalysis;
using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IInitializerModel
{
  ConstructorInfo Constructor { get; }
  
  IReadOnlyList<IPropertyModel> ConstructorProperties { get; }
  
  IReadOnlyList<IPropertyModel> InitializerProperties { get; }
}