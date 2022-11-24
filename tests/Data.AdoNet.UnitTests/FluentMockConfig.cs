using CodeArchitects.Platform.Data.AdoNet.Model;
using CodeArchitects.Platform.Data.AdoNet.Navigation;
using FluentMock;

[assembly: GenerateFluentMockFor(typeof(IEntityModel))]
[assembly: GenerateFluentMockFor(typeof(IPropertyModel))]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyPropertyModel))]
[assembly: GenerateFluentMockFor(typeof(IPrimaryKeyModel))]
[assembly: GenerateFluentMockFor(typeof(INavigationModel))]
[assembly: GenerateFluentMockFor(typeof(IKeyPair))]
[assembly: GenerateFluentMockFor(typeof(INavigationSpec))]
