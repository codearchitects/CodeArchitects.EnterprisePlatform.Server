using System;
using System.IO;
using System.Threading.Tasks;

namespace CodeArchitects.Platform.Infrastructure.Dapr.AspNetCore.Routing
{
  /// <summary>
  /// Exception that is thrown when the deserialization of a request body returns null.
  /// </summary>
  public class NullObjectDeserializedException : Exception
  {
    public NullObjectDeserializedException(string message, string? body)
      : base(message)
    {
      Base64Body = body;
    }

    /// <summary>
    /// The base-64 encoded body of the request that produced the exception.
    /// </summary>
    public string? Base64Body { get; }

    /// <summary>
    /// Creates a <see cref="NullObjectDeserializedException"/> instance.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="body">The request body encoded with base-64 digits.</param>
    /// <returns>A task that, when completed, returns the created instance.</returns>
    public static async Task<NullObjectDeserializedException> FromStreamAsync(string message, Stream? stream = null)
    {
      if (stream is null)
      {
        return new NullObjectDeserializedException(message, null);
      }
      try
      {
        using MemoryStream memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return new NullObjectDeserializedException(message, Convert.ToBase64String(memoryStream.ToArray()));
      }
      catch
      {
        return new NullObjectDeserializedException(message, null);
      }
    }
  }
}
