using CodeArchitects.Platform.Data.AdoNet.Model;
using FluentMock;

[assembly: GenerateFluentMockFor(typeof(IEntityModel))]
[assembly: GenerateFluentMockFor(typeof(IPropertyModel))]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyPropertyModel))]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyModel))]
[assembly: GenerateFluentMockFor(typeof(ISimpleNavigationModel))]
[assembly: GenerateFluentMockFor(typeof(ISkipNavigationModel))]
[assembly: GenerateFluentMockFor(typeof(IKeyPair))]
[assembly: GenerateFluentMockFor(typeof(IInitializerModel))]
