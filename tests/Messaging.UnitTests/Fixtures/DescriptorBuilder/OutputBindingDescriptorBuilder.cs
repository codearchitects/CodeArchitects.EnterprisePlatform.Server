using CodeArchitects.Platform.Messaging.Descriptors;
using Moq;

namespace CodeArchitects.Platform.Messaging.Fixtures.DescriptorBuilder;

internal class OutputBindingDescriptorBuilder : IOutputBindingDescriptorBuilder
{
  private readonly Mock<IOutputBindingDescriptor> _descriptorMock;

  public OutputBindingDescriptorBuilder(MockBehavior behavior)
  {
    _descriptorMock = new(behavior);
  }

  public IOutputBindingDescriptor Descriptor => _descriptorMock.Object;

  public IOutputBindingDescriptorBuilder SetMetadataType(Type metadataType)
  {
    _descriptorMock
      .Setup(x => x.MetadataType)
      .Returns(metadataType);
    return this;
  }

  public IOutputBindingDescriptorBuilder SetIsTypeFiltered(bool isTypeFiltered)
  {
    _descriptorMock
      .Setup(x => x.IsTypeFiltered)
      .Returns(isTypeFiltered);
    return this;
  }

  public IOutputBindingDescriptorBuilder SetMetadataObject(object metadataObject)
  {
    _descriptorMock
      .Setup(x => x.MetadataObject)
      .Returns(metadataObject);
    return this;
  }
}