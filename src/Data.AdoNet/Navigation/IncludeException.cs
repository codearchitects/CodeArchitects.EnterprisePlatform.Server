using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Data.AdoNet.Navigation;

[Serializable]
public class IncludeException : Exception
{
  public IncludeException()
    : base("Invalid include expression.")
  {
  }

  public IncludeException(string message)
    : base(message)
  {
  }

  public IncludeException(string message, Exception inner)
    : base(message, inner)
  {
  }

  protected IncludeException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}
