using CodeArchitects.Platform.Data.MongoDB.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;

namespace CodeArchitects.Platform.Data.MongoDB.Serialization;

/// <summary>
/// Registers the serialization settings the provider relies on.
/// </summary>
/// <remarks>
/// The BSON serializer registry and the convention registry are process-wide static state,
/// not per-container: a serializer cannot be replaced once registered, and a class map is frozen
/// the first time its type is serialized. Registration must therefore happen exactly once, while
/// the container is being composed and before any document is read or written.
/// </remarks>
internal static class MongoDBSerializationInitializer
{
  private const string ConventionPackName = "CodeArchitects.Platform.Data.MongoDB";
  private const string KeyConventionPackName = ConventionPackName + ".Keys";

  private static readonly object s_lock = new();

  private static readonly EntityKeyConvention s_keyConvention = new();

  private static GuidRepresentation? s_guidRepresentation;

  private static bool s_keyConventionRegistered;

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

      RegisterGuidSerializer((BsonSerializerRegistry)BsonSerializer.SerializerRegistry, guidRepresentation);

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

  /// <summary>
  /// Makes the driver store the key of every entity of the model in the <c>_id</c> element.
  /// </summary>
  /// <exception cref="InvalidOperationException">
  /// The class map of an entity was created before, and maps another member (or none) to <c>_id</c>.
  /// </exception>
  public static void MapKeys(IDataModel model)
  {
    if (model is null)
      throw new ArgumentNullException(nameof(model));

    lock (s_lock)
    {
      if (!s_keyConventionRegistered)
      {
        // A pack of its own, so ConfigureConventions cannot remove it by accident.
        ConventionRegistry.Register(KeyConventionPackName, new ConventionPack { s_keyConvention }, _ => true);
        s_keyConventionRegistered = true;
      }

      List<(IEntityModel Entity, Type DeclaringType)> keys = [];
      foreach (IEntityModel entity in model.Entities)
      {
        PropertyInfo property = entity.Type.GetProperty(entity.Key.Name, BindingFlags.Instance | BindingFlags.Public)
          ?? throw new InvalidOperationException($"The key property '{entity.Key.Name}' was not found on type '{entity.Type}'.");
        Type declaringType = property.DeclaringType!;

        if (!s_keyConvention.TryAddKey(declaringType, property.Name))
          throw new InvalidOperationException(
            $"The key of '{entity.Type}' is '{property.Name}', declared on '{declaringType}', where another entity " +
            "already has a different key. Mark the key with [BsonId].");

        keys.Add((entity, declaringType));
      }

      // The convention only applies to class maps created from now on: one created earlier
      // (registered by the application, or frozen by an earlier serialization) is checked instead.
      foreach ((IEntityModel entity, Type declaringType) in keys)
      {
        if (!BsonClassMap.IsClassMapRegistered(entity.Type) && !BsonClassMap.IsClassMapRegistered(declaringType))
          continue;

        string? mapped = BsonClassMap.LookupClassMap(entity.Type).IdMemberMap?.MemberName;
        if (mapped != entity.Key.Name)
          throw new InvalidOperationException(
            $"The class map of '{entity.Type}' maps {(mapped is null ? "no member" : $"'{mapped}'")} to _id, but its key is " +
            $"'{entity.Key.Name}'. The class map was created before AddData: register it after AddData, or map the " +
            $"key with MapIdMember or [BsonId].");
      }
    }
  }

  /// <summary>
  /// Registers the Guid serializer, or accepts the one the application already registered when it
  /// stores Guids with the same representation.
  /// </summary>
  /// <exception cref="InvalidOperationException">
  /// The registered serializer writes Guids as binary with another representation.
  /// </exception>
  internal static void RegisterGuidSerializer(BsonSerializerRegistry registry, GuidRepresentation guidRepresentation)
  {
    try
    {
      // Returns false when an equal serializer is already registered: nothing to do.
      registry.TryRegisterSerializer(typeof(Guid), new GuidSerializer(guidRepresentation));
    }
    catch (BsonSerializationException exception)
    {
      // A different serializer is registered. A non-binary or custom one is a deliberate choice of
      // the application, which the binary representation setting does not apply to: it is used as it is.
      if (registry.GetSerializer(typeof(Guid)) is GuidSerializer { Representation: BsonType.Binary } existing)
        throw new InvalidOperationException(
          $"A Guid serializer with GuidRepresentation.{existing.GuidRepresentation} is already registered, but the MongoDB " +
          $"provider is configured with GuidRepresentation.{guidRepresentation}. Call " +
          $"UseGuidRepresentation(GuidRepresentation.{existing.GuidRepresentation}) to match the registered serializer, or " +
          $"register a GuidSerializer with GuidRepresentation.{guidRepresentation}. A Guid serialized before AddData " +
          "registers a serializer too.",
          exception);
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
