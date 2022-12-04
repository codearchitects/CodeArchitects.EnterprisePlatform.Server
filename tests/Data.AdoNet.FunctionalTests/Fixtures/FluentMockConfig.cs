using CodeArchitects.Platform.Data.AdoNet.Model;
using FluentMock;

[assembly: GenerateFluentMockFor(typeof(IEntityModel))]
[assembly: GenerateFluentMockFor(typeof(IColumnModel))]
[assembly: GenerateFluentMockFor(typeof(IOrdinaryColumnModel), Ignore = new string[] { "GetValue", "SetValue", "Member" })]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyColumnModel), Ignore = new string[] { "GetValue", "SetValue", "Member" })]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyModel))]
[assembly: GenerateFluentMockFor(typeof(ISimpleAccessibleNavigationModel), Ignore = new string[] { "GetValue", "SetValue", "Member", "Inverse" })]
[assembly: GenerateFluentMockFor(typeof(ISkipAccessibleNavigationModel), Ignore = new string[] { "GetValue", "SetValue", "Member", "Inverse" })]
[assembly: GenerateFluentMockFor(typeof(IKeyPair))]
[assembly: GenerateFluentMockFor(typeof(IInitializerModel))]
