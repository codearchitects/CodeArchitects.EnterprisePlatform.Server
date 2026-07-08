using CodeArchitects.Platform.Messaging.Bindings;
using System.Diagnostics.CodeAnalysis;

namespace CodeArchitects.Platform.Messaging.Fixtures;

public interface IFakeOutputMetadata1 : IOutputMetadata
{
  string Name { get; }
}

[AttributeUsage(AttributeTargets.ReturnValue)]
public class FakeOutputBinding1Attribute : Attribute, IFakeOutputMetadata1
{
  public FakeOutputBinding1Attribute(string name)
  {
    Name = name;
  }

  public string Name { get; }

  public override bool Equals([NotNullWhen(true)] object? obj)
  {
    return obj is FakeOutputBinding1Attribute other && other.Name == Name;
  }

  public override int GetHashCode()
  {
    return Name.GetHashCode();
  }
}