using System.Collections.Generic;

namespace CodeArchitects.Platform.Infrastructure.Dapr.Messaging
{
  /// <summary>
  /// Metadata specific to the Dapr messaging infrastructure.
  /// </summary>
  public class DaprMetadata
  {
    public const string RawPayloadKey = "rawPayload";
    public const string TimeToLiveInSecondsKey = "ttlInSeconds";

    private readonly Dictionary<string, string> _metadataDictionary;

    /// <summary>
    /// Createsa new <see cref="DaprMetadata"/> instance.
    /// </summary>
    public DaprMetadata()
    {
      _metadataDictionary = new Dictionary<string, string>();
    }

    // TODO: Remove internal access modifier when Dapr will support IReadOnlyDictionary as metadata type.
    protected internal Dictionary<string, string> MetadataDictionary => _metadataDictionary;

    // TODO: Use when Dapr will support IReadOnlyDictionary as metadata type.
    internal IReadOnlyDictionary<string, string> AsDictionary() => _metadataDictionary;

    /// <summary>
    /// The name of the topic the message should be published to.
    /// </summary>
    public string? Topic { get; set; }

    /// <summary>
    /// Boolean to determine if Dapr should publish the event without wrapping it as CloudEvent as described
    /// <see href="https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-raw/">here</see>.
    /// </summary>
    public bool? RawPayload
    {
      get => _metadataDictionary.ContainsKey(RawPayloadKey) ? bool.Parse(_metadataDictionary[RawPayloadKey]) : null;
      set
      {
        if (value.HasValue)
        {
          _metadataDictionary[RawPayloadKey] = value.Value.ToString();
        }
      }
    }

    /// <summary>
    /// The number of seconds for the message to expire as described
    /// <see href="https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-message-ttl/">here</see>.
    /// </summary>
    public int? TimeToLiveInSeconds
    {
      get => _metadataDictionary.ContainsKey(TimeToLiveInSecondsKey) ? int.Parse(_metadataDictionary[TimeToLiveInSecondsKey]) : null;
      set
      {
        if (value.HasValue)
        {
          _metadataDictionary[TimeToLiveInSecondsKey] = value.Value.ToString();
        }
      }
    }
  }
}
