using CodeArchitects.Platform.Common.Utils;
using CodeArchitects.Platform.Emit.Reflection;
using CodeArchitects.Platform.Emit.Testing;
using CodeArchitects.Platform.GraphQL.ChilliCream.UnitTests.FluentMock;
using CodeArchitects.Platform.GraphQL.Model;
using CodeArchitects.Platform.GraphQL.Model.FluentMock;
using FluentAssertions;
using StrawberryShake;
using System.Reflection;

namespace CodeArchitects.Platform.GraphQL.ChilliCream;

public class VariableExtractorProviderTests
{
  private readonly Mock<IModel> _modelMock;

  public VariableExtractorProviderTests()
  {
    _modelMock = new(MockBehavior.Strict);

    _modelMock
      .Setup(x => x.GetVariables(typeof(Variables)))
      .Returns(ListBuilder<IVariable, VariableBuilder>.Build(_ => _
        .Add(_ => _
          .SetName(nameof(Variables.Obj1))
          .SetType(_ => _
            .SetClrType(typeof(string)))
          .SetClrProperty(typeof(Variables).GetRequiredProperty(nameof(Variables.Obj1))))
        .Add(_ => _
          .SetName(nameof(Variables.Obj2))
          .SetType(_ => _
            .SetClrType(typeof(int)))
          .SetClrProperty(typeof(Variables).GetRequiredProperty(nameof(Variables.Obj2))))
        .Add(_ => _
          .SetName(nameof(Variables.Obj3))
          .SetType(_ => _
            .SetClrType(typeof(int?)))
          .SetClrProperty(typeof(Variables).GetRequiredProperty(nameof(Variables.Obj3))))
        .Add(_ => _
          .SetName(nameof(Variables.File1))
          .SetType(_ => _
            .SetClrType(typeof(Upload)))
          .SetClrProperty(typeof(Variables).GetRequiredProperty(nameof(Variables.File1))))
        .Add(_ => _
          .SetName(nameof(Variables.File2))
          .SetType(_ => _
            .SetClrType(typeof(Upload?)))
          .SetClrProperty(typeof(Variables).GetRequiredProperty(nameof(Variables.File2))))));
  }

  [Fact]
  public void GetExtractor_ShouldProduceCorrectILOfPopulateMethod() // https://sharplab.io/#v2:C4LglgNgPgAgDAAhgRgCwG4CwAoHMDMSATAgMI4DeOCShKAbEqggAoD2ADgK4QCGwAUwBqvAE5heAIwgCAzgHUwwABYBRAG4DRATxVgAdgHMAFAEkAImADGwMG31jtAHhRwANAjaSAVgJsB+AD4EdTEJaQFLGzsHHQ8La1t7RxdkdwQAVQ4INl4AEyCEADNISMSYxw8RcSkZWRCw2rkASmoEKmwaGlCaiKik2O0AOgBBPLzjACIvb2RJjx7wuqGAeR9kZqxOrsWm/oqdUfGpmaJ5ht7lte8iTbbuxr7y5MOxiemffHPdiNlVz7u2y6CHuxVK+xewzeUxKMjmC0eywAYqUNltgbCytFIUd3pizgjLnIhiiZLd0QgAL44am4bAEYgIapLOSUNoM1z+BDXZDtBCGATAdAIWSC4W0mgMgzAbk+EgUflikVKiW0BDSrnXQgKgVC5V61UMrI5fIIUkCXk6pWig3swjG3IFM2leWKvU28U00F4QiyYCiLg2TLZR18ylAA==
  {
    // Arrange
    FakeILGeneratorProvider ilProvider = new();
    FakeILGenerator populateIL = ilProvider.AddGenerator();

    VariableExtractorProvider sut = new(new Synchronizer(), _modelMock.Object, ilProvider);

    // Act
    _ = sut.GetExtractor<Variables>();

    // Assert
    populateIL.VerifyNoLocals();
    populateIL.VerifyIL(_ => _
      .Ldarg_0()
      .Ldstr("obj1")
      .Ldarg_2()
      .Callvirt(typeof(Variables), $"get_{nameof(Variables.Obj1)}", Type.EmptyTypes)
      .Callvirt(typeof(IDictionary<string, object?>), nameof(IDictionary<string, object?>.Add), new[] { typeof(string), typeof(object) })
      .Ldarg_0()
      .Ldstr("obj2")
      .Ldarg_2()
      .Callvirt(typeof(Variables), $"get_{nameof(Variables.Obj2)}", Type.EmptyTypes)
      .Box(typeof(int))
      .Callvirt(typeof(IDictionary<string, object?>), nameof(IDictionary<string, object?>.Add), new[] { typeof(string), typeof(object) })
      .Ldarg_0()
      .Ldstr("obj3")
      .Ldarg_2()
      .Callvirt(typeof(Variables), $"get_{nameof(Variables.Obj3)}", Type.EmptyTypes)
      .Box(typeof(int?))
      .Callvirt(typeof(IDictionary<string, object?>), nameof(IDictionary<string, object?>.Add), new[] { typeof(string), typeof(object) })
      .Ldarg_1()
      .Ldstr("file1")
      .Ldarg_2()
      .Callvirt(typeof(Variables), $"get_{nameof(Variables.File1)}", Type.EmptyTypes)
      .Newobj(typeof(Upload?), new[] { typeof(Upload) })
      .Callvirt(typeof(IDictionary<string, Upload?>), nameof(IDictionary<string, Upload?>.Add), new[] { typeof(string), typeof(Upload?) })
      .Ldarg_1()
      .Ldstr("file2")
      .Ldarg_2()
      .Callvirt(typeof(Variables), $"get_{nameof(Variables.File2)}", Type.EmptyTypes)
      .Callvirt(typeof(IDictionary<string, Upload?>), nameof(IDictionary<string, Upload?>.Add), new[] { typeof(string), typeof(Upload?) })
      .Ret());
  }

  [Fact]
  public void Extractor_ShouldReturnCorrectDictionaries()
  {
    // Arrange
    VariableExtractorProvider sut = new(new Synchronizer(), _modelMock.Object, new ReflectionILGeneratorProvider());

    Variables variables = new()
    {
      Obj1 = "my-obj1",
      Obj2 = 12,
      Obj3 = null,
      File1 = new(new FakeStream(), "file1"),
      File2 = new(new FakeStream(), "file2")
    };

    VariableExtractor<Variables> extract = sut.GetExtractor<Variables>();

    // Act
    (IReadOnlyDictionary<string, object?> variableDictionary, IReadOnlyDictionary<string, Upload?> fileDictionary) = extract(variables);

    // Assert
    variableDictionary.Should().ContainKey("obj1").WhoseValue.Should().Be(variables.Obj1);
    variableDictionary.Should().ContainKey("obj2").WhoseValue.Should().Be(variables.Obj2);
    variableDictionary.Should().ContainKey("obj3").WhoseValue.Should().Be(variables.Obj3);
    fileDictionary.Should().ContainKey("file1").WhoseValue.Should().Be(variables.File1);
    fileDictionary.Should().ContainKey("file2").WhoseValue.Should().Be(variables.File2);
  }

  private class Variables
  {
    public string? Obj1 { get; set; }
    public int Obj2 { get; set; }
    public int? Obj3 { get; set; }
    public Upload File1 { get; set; }
    public Upload? File2 { get; set; }
  }

  private sealed class FakeStream : Stream
  {
    #region Not implemented

    public override bool CanRead => throw new NotImplementedException();

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => throw new NotImplementedException();

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush()
    {
      throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}