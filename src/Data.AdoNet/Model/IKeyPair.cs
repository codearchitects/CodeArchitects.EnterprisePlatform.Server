namespace CodeArchitects.Platform.Data.AdoNet.Model;

public interface IKeyPair
{
  IPropertyModel PrimaryKey { get; }
  
  IPropertyModel ForeignKey { get; }

  IPropertyModel FromProperty { get; }
  
  IPropertyModel ToProperty { get; }
}