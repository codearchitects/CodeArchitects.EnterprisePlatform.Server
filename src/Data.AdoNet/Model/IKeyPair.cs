namespace CodeArchitects.Platform.Data.AdoNet.Model;

internal interface IKeyPair
{
  IPropertyModel PrimaryKey { get; }
  IPropertyModel ForeignKey { get; }

  IPropertyModel FromProperty { get; }
  IPropertyModel ToProperty { get; }
}