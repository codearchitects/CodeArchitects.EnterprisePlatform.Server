using CodeArchitects.Platform.CodeAnalysis;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Experimental]
public interface IKeyPair
{
  IPropertyModel PrimaryKey { get; }
  
  IPropertyModel ForeignKey { get; }

  IPropertyModel FromProperty { get; }
  
  IPropertyModel ToProperty { get; }
}