using CodeArchitects.Platform.Data.AdoNet.Model;
using FluentMock;

[assembly: GenerateFluentMockFor(typeof(IEntityModel))]
[assembly: GenerateFluentMockFor(typeof(IColumnModel))]
[assembly: GenerateFluentMockFor(typeof(IOrdinaryColumnModel), Ignore = new string[] { "GetValue", "SetValue", "Member" })]
[assembly: GenerateFluentMockFor(typeof(IStandardPrimaryKeyColumnModel), Ignore = new string[] { "GetValue", "SetValue", "Member" })]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyModel))]
[assembly: GenerateFluentMockFor(typeof(IAccessibleSimpleNavigationModel), Ignore = new string[] { "GetValue", "SetValue", "Member", "Inverse" })]
[assembly: GenerateFluentMockFor(typeof(IAccessibleSkipNavigationModel), Ignore = new string[] { "GetValue", "SetValue", "Member", "Inverse" })]
[assembly: GenerateFluentMockFor(typeof(IKeyPair))]
[assembly: GenerateFluentMockFor(typeof(IInitializerModel))]
