using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Application.SignalR.Fakes
{
  public interface IFakeHub1
  {
    Task Method();
  }

  public class FakeHub1 : Hub<IFakeHub1>, IFakeHub1
  {
    public Task Method()
    {
      throw new NotImplementedException();
    }
  }

  public interface IFakeHub2
  {
    Task Method();
  }

  public class FakeHub2 : Hub<IFakeHub2>, IFakeHub2
  {
    public Task Method()
    {
      throw new NotImplementedException();
    }
  }
}
