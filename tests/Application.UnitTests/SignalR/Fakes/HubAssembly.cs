using System.Reflection;

namespace CodeArchitects.Platform.Application.SignalR.Fakes;

public static class HubAssembly
{
  public static readonly IReadOnlyDictionary<Type, Type> HubTypes = new Dictionary<Type, Type>
  {
    [HubInterface1Type] = HubClass1Type,
    [HubInterface2Type] = HubClass2Type
  };

  public static Assembly Instance
  {
    get
    {
      Mock<Assembly> assemblyMock = new(behavior: MockBehavior.Strict);
      assemblyMock
        .Setup(x => x.GetTypes())
        .Returns(new Type[] { HubInterface1Type, HubInterface2Type, HubClass1Type, HubClass2Type });
      assemblyMock
        .Setup(x => x.GetHashCode())
        .Returns(nameof(HubAssembly).GetHashCode());
      return assemblyMock.Object;
    }
  }

  public static Type HubInterface1Type => typeof(IFakeHub1);
  public static Type HubInterface2Type => typeof(IFakeHub2);
  public static Type HubClass1Type => typeof(FakeHub1);
  public static Type HubClass2Type => typeof(FakeHub2);
}
