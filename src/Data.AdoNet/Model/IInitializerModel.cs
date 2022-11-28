using System.Reflection;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IInitializerModel
{
  ConstructorInfo Constructor { get; }
  IReadOnlyList<IPropertyModel> ConstructorProperties { get; }
  IReadOnlyList<IPropertyModel> InitializerProperties { get; }
}