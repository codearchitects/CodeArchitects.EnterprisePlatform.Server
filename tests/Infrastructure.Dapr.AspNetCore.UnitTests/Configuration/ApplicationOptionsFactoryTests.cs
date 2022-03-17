using CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration.Fakes;
using CodeArchitects.Platform.Infrastructure.Dapr.Configuration;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Moq;
using System;
using System.IO;
using System.Text;
using Xunit;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Configuration;

public class ApplicationOptionsFactoryTests
{
  private readonly Mock<IFileProvider> _fileProviderMock;

  public ApplicationOptionsFactoryTests()
  {
    _fileProviderMock = new Mock<IFileProvider>(MockBehavior.Strict);
  }

  [Fact]
  public void FromFileProvider_ShouldReturnNull_IfDirectoryDoesNotExist()
  {
    // Arrange
    _fileProviderMock
      .Setup(x => x.GetDirectoryContents("/").Exists)
      .Returns(false);
    ApplicationOptionsFactory sut = new ApplicationOptionsFactory();

    // Act
    ApplicationOptions? options = sut.FromFileProvider(_fileProviderMock.Object);

    // Assert
    options.Should().BeNull();
  }

  [Fact]
  public void FromFileProvider_ShouldReturnNull_IfFilesDoNotExist()
  {
    // Arrange
    ApplicationOptions expectedOptions = new ApplicationOptions
    {
      MessageBusses = Array.Empty<string>(),
      StateStores = Array.Empty<string>()
    };
    Mock<IFileInfo> fileInfoMock = new Mock<IFileInfo>(MockBehavior.Strict);
    FakeDirectoryContents directoryContents = new FakeDirectoryContents(new[] { fileInfoMock.Object });
    _fileProviderMock
      .Setup(x => x.GetDirectoryContents("/"))
      .Returns(directoryContents);
    fileInfoMock
      .Setup(x => x.Exists)
      .Returns(false);
    ApplicationOptionsFactory sut = new ApplicationOptionsFactory();

    // Act
    ApplicationOptions? options = sut.FromFileProvider(_fileProviderMock.Object);

    // Assert
    options.Should().BeEquivalentTo(expectedOptions);
  }

  [Fact]
  public void FromFileProvider_ShouldReturnNull_IfFilesIsDirectory()
  {
    // Arrange
    ApplicationOptions expectedOptions = new ApplicationOptions
    {
      MessageBusses = Array.Empty<string>(),
      StateStores = Array.Empty<string>()
    };
    Mock<IFileInfo> fileInfoMock = new Mock<IFileInfo>(MockBehavior.Strict);
    FakeDirectoryContents directoryContents = new FakeDirectoryContents(new[] { fileInfoMock.Object });
    _fileProviderMock
      .Setup(x => x.GetDirectoryContents("/"))
      .Returns(directoryContents);
    fileInfoMock
      .Setup(x => x.Exists)
      .Returns(true);
    fileInfoMock
      .Setup(x => x.IsDirectory)
      .Returns(true);
    ApplicationOptionsFactory sut = new ApplicationOptionsFactory();

    // Act
    ApplicationOptions? options = sut.FromFileProvider(_fileProviderMock.Object);

    // Assert
    options.Should().BeEquivalentTo(expectedOptions);
  }

  [Fact]
  public void FromFileProvider_ShouldReadStateStoreComponents()
  {
    // Arrange
    IFileInfo component1 = CreateFakeFile(@"
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: statestore1
spec:
  type: state.redis
  metadata:
  - name: redisHost
    value: redis:6379
  - name: redisPassword
    value: """"
  - name: actorStateStore
    value: ""true""
");
    IFileInfo component2 = CreateFakeFile(@"
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: statestore2
spec:
  type: state.aws.dynamodb
  metadata:
  - name: table
    value: mytable
  - name: accessKey
    value: abcd
");

    ApplicationOptions expectedOptions = new ApplicationOptions
    {
      MessageBusses = Array.Empty<string>(),
      StateStores = new[] { "statestore1", "statestore2" }
    };

    FakeDirectoryContents directoryContents = new FakeDirectoryContents(new[] { component1, component2 });
    _fileProviderMock
      .Setup(x => x.GetDirectoryContents("/"))
      .Returns(directoryContents);

    ApplicationOptionsFactory sut = new ApplicationOptionsFactory();

    // Act
    ApplicationOptions? options = sut.FromFileProvider(_fileProviderMock.Object);

    // Assert
    options.Should().BeEquivalentTo(expectedOptions);
  }

  [Fact]
  public void FromFileProvider_ShouldReadMessageBusComponents()
  {
    // Arrange
    IFileInfo component1 = CreateFakeFile(@"
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: messagebus1
  namespace: default
spec:
  type: pubsub.redis
  version: v1
  metadata:
  - name: redisHost
    value: localhost:6379
  - name: redisPassword
    value: KeFg23
");
    IFileInfo component2 = CreateFakeFile(@"
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: messagebus2
  namespace: default
spec:
  type: pubsub.rabbitmq
  version: v1
  metadata:
  - name: host
    value: ""amqp://rabbitmq:5672""
");

    ApplicationOptions expectedOptions = new ApplicationOptions
    {
      MessageBusses = new[] { "messagebus1", "messagebus2" },
      StateStores = Array.Empty<string>()
    };

    FakeDirectoryContents directoryContents = new FakeDirectoryContents(new[] { component1, component2 });
    _fileProviderMock
      .Setup(x => x.GetDirectoryContents("/"))
      .Returns(directoryContents);

    ApplicationOptionsFactory sut = new ApplicationOptionsFactory();

    // Act
    ApplicationOptions? options = sut.FromFileProvider(_fileProviderMock.Object);

    // Assert
    options.Should().BeEquivalentTo(expectedOptions);
  }

  private static IFileInfo CreateFakeFile(string content)
  {
    Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

    Mock<IFileInfo> fileInfoMock = new Mock<IFileInfo>(MockBehavior.Strict);
    fileInfoMock
      .Setup(x => x.Exists)
      .Returns(true);
    fileInfoMock
      .Setup(x => x.IsDirectory)
      .Returns(false);
    fileInfoMock
      .Setup(x => x.CreateReadStream())
      .Returns(stream);

    return fileInfoMock.Object;
  }
}
