using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Serializable]
public class ModelConfigurationException : Exception
{
  public ModelConfigurationException()
    : base("There was an error during the data model construction.")
  {
  }

  public ModelConfigurationException(string message)
    : base(message)
  {
  }

  public ModelConfigurationException(string message, Exception inner)
    : base(message, inner)
  {
  }

  protected ModelConfigurationException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}