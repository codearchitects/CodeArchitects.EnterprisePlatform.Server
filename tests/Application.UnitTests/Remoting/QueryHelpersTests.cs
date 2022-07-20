using FluentAssertions;
using System;
using static CodeArchitects.Platform.Application.Remoting.QueryHelpersFixtures;

namespace CodeArchitects.Platform.Application.Remoting;

public class QueryHelpersTests
{
  [Theory]
  [SimpleTypesOrArraysData]
  public void IsSimpleType_ShouldBeTrue_ForSimpleTypesAndArrays(Type type)
  {
    // Arrange

    // Act
    bool isSimpleType = QueryHelpers.IsSimpleType(type);

    // Assert
    isSimpleType.Should().BeTrue();
  }

  [Fact]
  public void IsSimpleType_ShouldBeFalse_ForComplexTypes()
  {
    // Arrange
    var obj = new
    {
      Field1 = "field1",
      Field2 = "field2"
    };

    // Act
    bool isSimpleType = QueryHelpers.IsSimpleType(obj.GetType());

    // Assert
    isSimpleType.Should().BeFalse();
  }

  [Fact]
  public void AddQueryToUrl_ShouldEncode1ObjectInUrl()
  {
    // Arrange
    const string baseUrl = "https://www.codearchitects.com";
    const string expectedQuery = "?Complex.Inner.Field1=myField1&Complex.Inner.Field2=2&Complex.Array=element1&Complex.Array=element2&Simple=a250b9ae-49b4-4440-93e9-b3f043f92413";
    var query = new
    {
      Complex = new
      {
        Inner = new
        {
          Field1 = "myField1",
          Field2 = 2
        },
        Array = new string[] { "element1", "element2" }
      },
      Simple = Guid.Parse("a250b9ae-49b4-4440-93e9-b3f043f92413")
    };

    // Act
    string url = QueryHelpers.AddQueryToUrl(baseUrl, query);

    // Assert
    url.Should().Be($"{baseUrl}{expectedQuery}");
  }

  [Fact]
  public void AddQueryToUrl_ShouldEncode2ObjectsInUrl()
  {
    // Arrange
    const string baseUrl = "https://www.codearchitects.com";
    const string expectedQuery = "?Complex.Inner.Field1=myField1&Complex.Inner.Field2=2&Complex.Array=element1&Complex.Array=element2&Simple=a250b9ae-49b4-4440-93e9-b3f043f92413&Field2=field2";
    var query1 = new
    {
      Complex = new
      {
        Inner = new
        {
          Field1 = "myField1",
          Field2 = 2
        },
        Array = new string[] { "element1", "element2" }
      },
      Simple = Guid.Parse("a250b9ae-49b4-4440-93e9-b3f043f92413")
    };
    var query2 = new
    {
      Field2 = "field2"
    };

    // Act
    string url = QueryHelpers.AddQueryToUrl(baseUrl, query1, query2);

    // Assert
    url.Should().Be($"{baseUrl}{expectedQuery}");
  }

  [Fact]
  public void AddQueryToUrl_ShouldEncode3ObjectsInUrl()
  {
    // Arrange
    const string baseUrl = "https://www.codearchitects.com";
    const string expectedQuery = "?Field1=field1&Complex.Inner.Field1=myField1&Complex.Inner.Field2=2&Complex.Array=element1&Complex.Array=element2&Simple=a250b9ae-49b4-4440-93e9-b3f043f92413&Field3=field3";
    var query1 = new
    {
      Field1 = "field1"
    };
    var query2 = new
    {
      Complex = new
      {
        Inner = new
        {
          Field1 = "myField1",
          Field2 = 2
        },
        Array = new string[] { "element1", "element2" }
      },
      Simple = Guid.Parse("a250b9ae-49b4-4440-93e9-b3f043f92413")
    };
    var query3 = new
    {
      Field3 = "field3"
    };

    // Act
    string url = QueryHelpers.AddQueryToUrl(baseUrl, query1, query2, query3);

    // Assert
    url.Should().Be($"{baseUrl}{expectedQuery}");
  }

  [Fact]
  public void AddQueryToUrl_ShouldEncodeNObjectsInUrl()
  {
    // Arrange
    const string baseUrl = "https://www.codearchitects.com";
    const string expectedQuery = "?Field1=field1&Field2=field2&Field3=field3&Field4=field4";
    var query1 = new
    {
      Field1 = "field1"
    };
    var query2 = new
    {
      Field2 = "field2"
    };
    var query3 = new
    {
      Field3 = "field3"
    };
    var query4 = new
    {
      Field4 = "field4"
    };

    // Act
    string url = QueryHelpers.AddQueryToUrl(baseUrl, query1, query2, query3, query4);

    // Assert
    url.Should().Be($"{baseUrl}{expectedQuery}");
  }
}
