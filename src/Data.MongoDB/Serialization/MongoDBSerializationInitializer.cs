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

  private static readonly object s_lock = new();

  private static GuidRepresentation? s_guidRepresentation;

  public static void EnsureInitialized(
    GuidRepresentation guidRepresentation = GuidRepresentation.Standard,
    IReadOnlyList<Action<ConventionPack>>? conventionConfigurators = null)
  {
    lock (s_lock)
    {
      if (s_guidRepresentation is GuidRepresentation applied)
      {
        EnsureCompatible(applied, guidRepresentation, conventionConfigurators);
        return;
      }

      BsonSerializer.RegisterSerializer(new GuidSerializer(guidRepresentation));

      ConventionPack pack =
      [
        new IgnoreExtraElementsConvention(true)
      ];

      if (conventionConfigurators is not null)
      {
        foreach (Action<ConventionPack> configure in conventionConfigurators)
        {
          configure(pack);
        }
      }

      ConventionRegistry.Register(ConventionPackName, pack, _ => true);

      s_guidRepresentation = guidRepresentation;
    }
  }

  private static void EnsureCompatible(
    GuidRepresentation applied,
    GuidRepresentation requested,
    IReadOnlyList<Action<ConventionPack>>? conventionConfigurators)
  {
    if (requested != applied)
      throw new InvalidOperationException(
        $"The MongoDB serialization was already initialized with GuidRepresentation.{applied}: GuidRepresentation.{requested} cannot be applied.");

    if (conventionConfigurators is { Count: > 0 })
      throw new InvalidOperationException(
        "The MongoDB conventions were already registered by a previous AddData call and cannot be " +
        "changed, because the convention registry is shared by the whole process. Call " +
        "ConfigureConventions only in the first AddData call.");
  }
}
