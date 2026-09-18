using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace CodeArchitects.Platform.Data.MongoDB.Serialization;

/// <summary>
/// Registers the serialization settings the provider relies on.
/// </summary>
/// <remarks>
/// The BSON serializer registry and the convention registry are process-wide static state,
/// not per-container: registering a serializer twice throws, and a class map is frozen the
/// first time its type is serialized. Registration must therefore happen exactly once, while
/// the container is being composed and before any document is read or written.
/// </remarks>
internal static class MongoDBSerializationInitializer
{
  private const string ConventionPackName = "CodeArchitects.Platform.Data.MongoDB";

  private static int s_initialized;

  public static void EnsureInitialized(
    GuidRepresentation guidRepresentation = GuidRepresentation.Standard,
    IReadOnlyList<Action<ConventionPack>>? conventionConfigurators = null)
  {
    // AddData may be called more than once (multiple databases): only the first call registers.
    if (Interlocked.CompareExchange(ref s_initialized, 1, 0) != 0)
      return;

    // Driver 3.x has no implicit Guid representation: without this, serializing a Guid key
    // throws "GuidSerializer cannot serialize a Guid when GuidRepresentation is Unspecified".
    BsonSerializer.RegisterSerializer(new GuidSerializer(guidRepresentation));

    ConventionPack pack = new()
    {
      new IgnoreExtraElementsConvention(true)
    };

    if (conventionConfigurators is not null)
    {
      foreach (Action<ConventionPack> configure in conventionConfigurators)
      {
        configure(pack);
      }
    }

    ConventionRegistry.Register(ConventionPackName, pack, _ => true);
  }
}
