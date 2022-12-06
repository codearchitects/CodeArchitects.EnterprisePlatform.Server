using System.Runtime.Serialization;

namespace CodeArchitects.Platform.Data.AdoNet.Model;

[Serializable]
public class ModelConstructionException : Exception
{
  public ModelConstructionException()
    : base("There was an error during the data model construction.")
  {
  }

  public ModelConstructionException(string message)
    : base(message)
  {
  }

  public ModelConstructionException(string message, Exception inner)
    : base(message, inner)
  {
  }

  protected ModelConstructionException(SerializationInfo info, StreamingContext context)
    : base(info, context)
  {
  }
}