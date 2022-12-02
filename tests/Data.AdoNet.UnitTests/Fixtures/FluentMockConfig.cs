using CodeArchitects.Platform.Data.AdoNet.Model;
using FluentMock;

[assembly: GenerateFluentMockFor(typeof(IEntityModel))]
[assembly: GenerateFluentMockFor(typeof(IPropertyModel))]
[assembly: GenerateFluentMockFor(typeof(IOrdinaryPropertyModel), Ignore = new string[] { "GetValue", "SetValue", "Member" })]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyPropertyModel), Ignore = new string[] { "GetValue", "SetValue", "Member" })]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyModel))]
[assembly: GenerateFluentMockFor(typeof(ISimpleAccessibleNavigationModel), Ignore = new string[] { "GetValue", "SetValue", "Member", "Inverse" })]
[assembly: GenerateFluentMockFor(typeof(ISkipAccessibleNavigationModel), Ignore = new string[] { "GetValue", "SetValue", "Member", "Inverse" })]
[assembly: GenerateFluentMockFor(typeof(IKeyPair))]
[assembly: GenerateFluentMockFor(typeof(IInitializerModel))]
